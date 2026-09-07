using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;
using Tahila.Infrastructure.Data;

namespace Tahila.Web.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر پرداخت آنلاین رو مدیریت میکنه
// دو مرحله داره: ۱) درخواست پرداخت  ۲) تأیید پرداخت بعد از بازگشت از بانک
// ─────────────────────────────────────────────────────────────────────────────
[Authorize] // همه اکشن‌های این کنترلر نیاز به لاگین دارن — مگه استثنا بدیم
public class PaymentController : Controller
{
    // سرویس پرداخت — ارتباط با درگاه بانکی (مثل زرین‌پال)
    private readonly IPaymentService _paymentService;

    // برای گرفتن اطلاعات کاربر لاگین‌کرده
    private readonly UserManager<ApplicationUser> _userManager;

    // سازنده
    public PaymentController(IPaymentService paymentService, UserManager<ApplicationUser> userManager)
    {
        _paymentService = paymentService;
        _userManager    = userManager;
    }

    // ── مرحله ۱: درخواست پرداخت ─────────────────────────────────────────────
    // کاربر روی "خرید پکیج X" کلیک میکنه، اینجا میاد
    // آدرس: /payment/request/{planId}
    [HttpGet("payment/request/{planId}")]
    public new async Task<IActionResult> Request(int planId)
    {
        // کاربر لاگین‌کرده رو پیدا میکنیم
        var user = await _userManager.GetUserAsync(User);

        // اگه به هر دلیلی پیدا نشد، بفرستش لاگین
        if (user == null) return RedirectToAction("Login", "Account");

        try
        {
            // از سرویس پرداخت میخوایم لینک درگاه بانک رو بسازه
            // این لینک شامل شماره تراکنش و مبلغ هست
            var redirectUrl = await _paymentService.RequestPaymentAsync(planId, user.Id);

            // کاربر رو به درگاه بانک هدایت میکنیم
            return Redirect(redirectUrl);
        }
        catch (Exception ex)
        {
            // اگه مشکلی پیش اومد (مثلاً درگاه در دسترس نیست)، برمیگردونیم لیست پکیج‌ها
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "Plans");
        }
    }

    // ── مرحله ۲: تأیید پرداخت ───────────────────────────────────────────────
    // بعد از پرداخت، بانک کاربر رو به این آدرس برمیگردونه
    // آدرس: /payment/verify?authority=xxx&status=OK
    [HttpGet("payment/verify")]
    [AllowAnonymous] // این یکی نیاز به لاگین نداره چون بانک کاربر رو redirect میکنه
    public async Task<IActionResult> Verify(string authority, string status)
    {
        // کاربر رو پیدا میکنیم
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        // از سرویس میخوایم پرداخت رو تأیید کنه
        // authority = کد یکتای تراکنش از بانک
        // status = OK یا NOK (موفق یا ناموفق)
        var success = await _paymentService.VerifyPaymentAsync(authority, status, user.Id);

        // عنوان صفحه بر اساس نتیجه
        ViewData["Title"] = success ? "پرداخت موفق" : "پرداخت ناموفق";
        ViewBag.Success   = success;
        ViewBag.Authority = authority; // کد رهگیری برای نمایش به کاربر

        return View();
    }
}
