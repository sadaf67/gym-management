using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;

namespace Tahila.Web.Areas.Admin.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر صفحه اصلی پنل ادمین رو مدیریت میکنه
// اون صفحه‌ای که ادمین اول وارد میشه و آمار کلی رو میبینه
// آدرس: /Admin/Dashboard
// ─────────────────────────────────────────────────────────────────────────────
[Area("Admin")]
[Authorize(Roles = "Admin")] // فقط ادمین میتونه وارد بشه
public class DashboardController : Controller
{
    // سرویس پرداخت — برای نمایش درآمد و آخرین تراکنش‌ها
    private readonly IPaymentService _paymentService;

    // سرویس مربیان — برای تعداد مربیان
    private readonly ICoachService _coachService;

    // سرویس کلاس‌ها — برای تعداد کلاس‌های فعال
    private readonly IClassService _classService;

    // سرویس پکیج‌ها — برای تعداد پکیج‌های عضویت
    private readonly IPlanService _planService;

    // سازنده
    public DashboardController(
        IPaymentService paymentService,
        ICoachService coachService,
        IClassService classService,
        IPlanService planService)
    {
        _paymentService = paymentService;
        _coachService   = coachService;
        _classService   = classService;
        _planService    = planService;
    }

    // ── صفحه اصلی داشبورد ───────────────────────────────────────────────────
    public async Task<IActionResult> Index()
    {
        // همه پرداخت‌ها رو بیار
        var payments = await _paymentService.GetAllPaymentsAsync();

        // همه مربیان رو بیار
        var coaches  = await _coachService.GetAllAsync();

        // همه کلاس‌ها رو بیار
        var classes  = await _classService.GetAllAsync();

        // همه پکیج‌ها رو بیار
        var plans    = await _planService.GetAllAsync();

        // مجموع درآمد = جمع مبالغ پرداخت‌های موفق
        // Where: فقط پرداخت‌های موفق حساب بشن (نه ناموفق یا در انتظار)
        ViewBag.TotalRevenue = payments
            .Where(p => p.Status == Domain.Enums.PaymentStatus.Success)
            .Sum(p => p.Amount);

        // تعداد مربیان فعال
        ViewBag.TotalCoaches = coaches.Count();

        // تعداد کلاس‌های موجود
        ViewBag.TotalClasses = classes.Count();

        // تعداد پکیج‌های موجود
        ViewBag.TotalPlans   = plans.Count();

        // ۵ پرداخت اخیر — برای جدول "آخرین تراکنش‌ها"
        ViewBag.RecentPayments = payments.Take(5).ToList();

        return View();
    }
}
