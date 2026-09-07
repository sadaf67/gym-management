using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;

namespace Tahila.Web.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر صفحه "پکیج‌های عضویت" رو نمایش میده
// آدرس: /Plans
// کاربر اینجا قیمت‌ها و امکانات هر پکیج رو میبینه و انتخاب میکنه
// ─────────────────────────────────────────────────────────────────────────────
public class PlansController : Controller
{
    // سرویس پکیج‌ها — برای گرفتن لیست پکیج‌های فعال
    private readonly IPlanService _planService;

    // سرویس پرداخت — برای شروع فرآیند خرید (آماده برای آینده)
    private readonly IPaymentService _paymentService;

    public PlansController(IPlanService planService, IPaymentService paymentService)
    { _planService = planService; _paymentService = paymentService; }

    // ── صفحه پکیج‌ها ────────────────────────────────────────────────────────
    // پکیج‌های فعال رو از دیتابیس میگیره
    // View این پکیج‌ها رو به صورت کارت نشون میده — با قیمت و امکانات
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "پلن‌های عضویت";
        return View(await _planService.GetActivePlansAsync());
    }
}
