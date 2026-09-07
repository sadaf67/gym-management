using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Domain.Enums;
using Tahila.Domain.Interfaces;
using Tahila.Infrastructure.Data;

namespace Tahila.Infrastructure.Payments;

public class ZarinPalService : IPaymentService
{
    private readonly IRepository<Payment> _paymentRepo;
    private readonly IRepository<Plan> _planRepo;
    private readonly IRepository<Subscription> _subscriptionRepo;
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    private string MerchantId => _config["ZarinPal:MerchantId"] ?? "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX";
    private string CallbackUrl => _config["ZarinPal:CallbackUrl"] ?? "https://localhost/payment/verify";
    private bool IsSandbox => bool.Parse(_config["ZarinPal:Sandbox"] ?? "true");

    private string RequestUrl => IsSandbox
        ? "https://sandbox.zarinpal.com/pg/v4/payment/request.json"
        : "https://payment.zarinpal.com/pg/v4/payment/request.json";

    private string VerifyUrl => IsSandbox
        ? "https://sandbox.zarinpal.com/pg/v4/payment/verify.json"
        : "https://payment.zarinpal.com/pg/v4/payment/verify.json";

    private string GatewayUrl => IsSandbox
        ? "https://sandbox.zarinpal.com/pg/StartPay/"
        : "https://www.zarinpal.com/pg/StartPay/";

    public ZarinPalService(
        IRepository<Payment> paymentRepo,
        IRepository<Plan> planRepo,
        IRepository<Subscription> subscriptionRepo,
        IConfiguration config,
        HttpClient httpClient)
    {
        _paymentRepo = paymentRepo;
        _planRepo = planRepo;
        _subscriptionRepo = subscriptionRepo;
        _config = config;
        _httpClient = httpClient;
    }

    public async Task<string> RequestPaymentAsync(int planId, string userId)
    {
        var plan = await _planRepo.GetByIdAsync(planId)
            ?? throw new Exception("پلن مورد نظر یافت نشد.");

        var subscription = await _subscriptionRepo.AddAsync(new Subscription
        {
            UserId = userId,
            PlanId = planId,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(plan.DurationDays),
            Status = SubscriptionStatus.Pending
        });

        var body = new
        {
            merchant_id = MerchantId,
            amount = (long)plan.Price,
            description = $"خرید اشتراک {plan.Name} - باشگاه تهیلا",
            callback_url = CallbackUrl
        };

        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(RequestUrl, content);
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(json);

        var code = result.GetProperty("data").GetProperty("code").GetInt32();
        if (code != 100)
            throw new Exception($"خطا در اتصال به درگاه پرداخت. کد: {code}");

        var authority = result.GetProperty("data").GetProperty("authority").GetString()!;

        await _paymentRepo.AddAsync(new Payment
        {
            UserId = userId,
            SubscriptionId = subscription.Id,
            Amount = plan.Price,
            Status = PaymentStatus.Pending,
            Gateway = PaymentGateway.ZarinPal,
            Authority = authority
        });

        return GatewayUrl + authority;
    }

    public async Task<bool> VerifyPaymentAsync(string authority, string status, string userId)
    {
        if (status != "OK") return false;

        var payment = _paymentRepo.Query()
            .Include(p => p.Subscription).ThenInclude(s => s.Plan)
            .FirstOrDefault(p => p.Authority == authority && p.UserId == userId);

        if (payment == null) return false;

        var body = new
        {
            merchant_id = MerchantId,
            amount = (long)payment.Amount,
            authority
        };

        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(VerifyUrl, content);
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(json);

        var code = result.GetProperty("data").GetProperty("code").GetInt32();
        if (code != 100 && code != 101) return false;

        var refId = result.GetProperty("data").GetProperty("ref_id").GetInt64().ToString();

        payment.Status = PaymentStatus.Success;
        payment.RefId = refId;
        payment.PaidAt = DateTime.Now;
        await _paymentRepo.UpdateAsync(payment);

        payment.Subscription.Status = SubscriptionStatus.Active;
        await _subscriptionRepo.UpdateAsync(payment.Subscription);

        return true;
    }

    public async Task<IEnumerable<Payment>> GetUserPaymentsAsync(string userId)
        => _paymentRepo.Query().Include(p => p.Subscription).ThenInclude(s => s.Plan)
            .Where(p => p.UserId == userId).OrderByDescending(p => p.CreatedAt).ToList();

    public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        => _paymentRepo.Query().Include(p => p.Subscription).ThenInclude(s => s.Plan)
            .OrderByDescending(p => p.CreatedAt).ToList();
}
