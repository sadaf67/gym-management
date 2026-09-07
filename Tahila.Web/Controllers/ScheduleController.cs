using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;

namespace Tahila.Web.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر صفحه "برنامه هفتگی باشگاه" رو نمایش میده
// آدرس: /Schedule
// کاربر اینجا میبینه هر روز چه کلاس‌هایی و چه ساعتی برگزار میشه
// ─────────────────────────────────────────────────────────────────────────────
public class ScheduleController : Controller
{
    // سرویس برنامه‌ها — برای گرفتن جدول هفتگی
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService) => _scheduleService = scheduleService;

    // ── صفحه برنامه هفتگی ───────────────────────────────────────────────────
    // همه برنامه‌های فعال رو میگیره — همراه اطلاعات کلاس و مربی
    // View اینها رو توی یه جدول هفتگی نشون میده
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "برنامه هفتگی";
        return View(await _scheduleService.GetAllWithClassAsync());
    }
}
