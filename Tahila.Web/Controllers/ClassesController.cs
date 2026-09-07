using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;

namespace Tahila.Web.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر صفحه "کلاس‌های ورزشی" رو نمایش میده
// آدرس: /Classes
// کاربر اینجا میتونه ببینه باشگاه چه کلاس‌هایی داره — یوگا، بوکس، ...
// ─────────────────────────────────────────────────────────────────────────────
public class ClassesController : Controller
{
    // سرویس کلاس‌ها — برای گرفتن لیست کلاس‌های فعال
    private readonly IClassService _classService;

    // سرویس مربیان — فعلاً استفاده نمیشه ولی آماده‌ست برای آینده
    private readonly ICoachService _coachService;

    // سازنده — ASP.NET خودکار این سرویس‌ها رو inject میکنه
    public ClassesController(IClassService classService, ICoachService coachService)
    { _classService = classService; _coachService = coachService; }

    // ── صفحه اصلی کلاس‌ها ───────────────────────────────────────────────────
    // لیست همه کلاس‌های فعال رو به View میفرسته
    public async Task<IActionResult> Index()
    {
        // عنوان صفحه — توی Layout استفاده میشه
        ViewData["Title"] = "کلاس‌ها";

        // کلاس‌های فعال رو از دیتابیس بگیر
        var classes = await _classService.GetActiveClassesAsync();
        return View(classes);
    }
}
