using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;

namespace Tahila.Web.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر صفحه "مربیان ما" رو نمایش میده
// آدرس: /Coaches
// کاربر اینجا میتونه همه مربیان باشگاه رو با عکس و تخصصشون ببینه
// ─────────────────────────────────────────────────────────────────────────────
public class CoachesController : Controller
{
    // سرویس مربیان — برای گرفتن لیست مربیان فعال
    private readonly ICoachService _coachService;

    // سازنده — مختصر نوشته شده: مستقیم assign میکنیم
    public CoachesController(ICoachService coachService) => _coachService = coachService;

    // ── صفحه مربیان ─────────────────────────────────────────────────────────
    // لیست مربیان فعال رو از دیتابیس میگیره و به View میفرسته
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "مربیان";
        return View(await _coachService.GetActiveCoachesAsync());
    }
}
