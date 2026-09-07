using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Domain.Enums;

namespace Tahila.Web.Areas.Admin.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر "برنامه هفتگی باشگاه" رو در پنل ادمین مدیریت میکنه
// آدرس: /Admin/Schedule
// ادمین میتونه تعریف کنه هر کلاس چه روزی و چه ساعتی برگزار میشه
// ─────────────────────────────────────────────────────────────────────────────
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ScheduleController : Controller
{
    // سرویس برنامه هفتگی
    private readonly IScheduleService _service;

    // سرویس کلاس‌ها — برای نمایش لیست کلاس‌ها توی dropdown
    private readonly IClassService _classService;

    public ScheduleController(IScheduleService service, IClassService classService)
    { _service = service; _classService = classService; }

    // ── لیست همه برنامه‌ها ──────────────────────────────────────────────────
    // همراه اطلاعات کلاس و مربی
    public async Task<IActionResult> Index()
    { ViewData["PageTitle"] = "برنامه هفتگی"; return View(await _service.GetAllWithClassAsync()); }

    // ── آماده کردن dropdown‌ها ────────────────────────────────────────────────
    // قبل از فرم Create و Edit این رو صدا میزنیم
    private async Task PrepareViewBag()
    {
        // لیست کلاس‌ها — برای انتخاب اینکه این برنامه مال کدوم کلاسه
        var classes = await _classService.GetAllAsync();
        ViewBag.Classes = new SelectList(classes, "Id", "Name");

        // لیست روزهای هفته به فارسی
        ViewBag.Days = new SelectList(new[] {
            new { Id = 0, Name = "شنبه" }, new { Id = 1, Name = "یکشنبه" },
            new { Id = 2, Name = "دوشنبه" }, new { Id = 3, Name = "سه‌شنبه" },
            new { Id = 4, Name = "چهارشنبه" }, new { Id = 5, Name = "پنجشنبه" },
            new { Id = 6, Name = "جمعه" }
        }, "Id", "Name");
    }

    // ── فرم برنامه جدید ─────────────────────────────────────────────────────
    public async Task<IActionResult> Create()
    { ViewData["PageTitle"] = "برنامه جدید"; await PrepareViewBag(); return View(new Schedule()); }

    // ── ذخیره برنامه جدید ───────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Schedule model)
    {
        ViewData["PageTitle"] = "برنامه جدید";
        await _service.CreateAsync(model);
        TempData["Success"] = "برنامه اضافه شد.";
        return RedirectToAction(nameof(Index));
    }

    // ── فرم ویرایش برنامه ───────────────────────────────────────────────────
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["PageTitle"] = "ویرایش برنامه";
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        await PrepareViewBag();
        return View(item);
    }

    // ── ذخیره تغییرات برنامه ────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Schedule model)
    {
        ViewData["PageTitle"] = "ویرایش برنامه";
        await _service.UpdateAsync(model);
        TempData["Success"] = "برنامه ویرایش شد.";
        return RedirectToAction(nameof(Index));
    }

    // ── حذف برنامه ──────────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    { await _service.DeleteAsync(id); TempData["Success"] = "برنامه حذف شد."; return RedirectToAction(nameof(Index)); }
}
