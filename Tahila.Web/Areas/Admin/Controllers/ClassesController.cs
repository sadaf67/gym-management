using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Domain.Enums;
using Tahila.Infrastructure.FileUpload;

namespace Tahila.Web.Areas.Admin.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر "کلاس‌های ورزشی" رو در پنل ادمین مدیریت میکنه
// آدرس: /Admin/Classes
// ادمین کلاس اضافه میکنه، مربی بهش اختصاص میده، و تعیین میکنه برای کدوم جنس
// ─────────────────────────────────────────────────────────────────────────────
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ClassesController : Controller
{
    // سرویس کلاس‌ها — برای عملیات CRUD
    private readonly IClassService _service;

    // سرویس مربیان — برای نمایش لیست مربیان توی dropdown
    private readonly ICoachService _coachService;

    // سرویس آپلود — برای ذخیره عکس کلاس
    private readonly FileUploadService _fileUpload;

    public ClassesController(IClassService service, ICoachService coachService, FileUploadService fileUpload)
    { _service = service; _coachService = coachService; _fileUpload = fileUpload; }

    // ── لیست همه کلاس‌ها ────────────────────────────────────────────────────
    public async Task<IActionResult> Index()
    { ViewData["PageTitle"] = "مدیریت کلاس‌ها"; return View(await _service.GetAllAsync()); }

    // ── آماده کردن داده‌های dropdown ─────────────────────────────────────────
    // این متد رو قبل از نمایش فرم Create و Edit صدا میزنیم
    // SelectList یه لیست برای dropdown میسازه
    private async Task PrepareViewBag()
    {
        // لیست مربیان — Id و FullName — برای انتخاب مربی کلاس
        var coaches = await _coachService.GetAllAsync();
        ViewBag.Coaches = new SelectList(coaches, "Id", "FullName");

        // لیست جنسیت — برای تعیین این کلاس برای کیه
        ViewBag.Genders = new SelectList(new[] {
            new { Id = 0, Name = "همه" },
            new { Id = 1, Name = "آقایان" },
            new { Id = 2, Name = "بانوان" }
        }, "Id", "Name");
    }

    // ── فرم کلاس جدید ───────────────────────────────────────────────────────
    public async Task<IActionResult> Create()
    {
        ViewData["PageTitle"] = "کلاس جدید";
        await PrepareViewBag(); // dropdown‌ها رو آماده کن
        return View(new GymClass());
    }

    // ── ذخیره کلاس جدید ─────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GymClass model, IFormFile? imageFile)
    {
        ViewData["PageTitle"] = "کلاس جدید";
        try
        {
            // اگه عکس کلاس آپلود شده، ذخیره کن
            if (imageFile != null) model.ImagePath = await _fileUpload.UploadImageAsync(imageFile, "images/classes");
            await _service.CreateAsync(model);
            TempData["Success"] = "کلاس با موفقیت اضافه شد.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) { await PrepareViewBag(); TempData["Error"] = ex.Message; return View(model); }
    }

    // ── فرم ویرایش کلاس ─────────────────────────────────────────────────────
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["PageTitle"] = "ویرایش کلاس";
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        await PrepareViewBag(); // dropdown‌ها رو آماده کن
        return View(item);
    }

    // ── ذخیره تغییرات کلاس ──────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(GymClass model, IFormFile? imageFile)
    {
        ViewData["PageTitle"] = "ویرایش کلاس";
        try
        {
            if (imageFile != null) model.ImagePath = await _fileUpload.UploadImageAsync(imageFile, "images/classes");
            await _service.UpdateAsync(model);
            TempData["Success"] = "کلاس با موفقیت ویرایش شد.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) { await PrepareViewBag(); TempData["Error"] = ex.Message; return View(model); }
    }

    // ── حذف کلاس ────────────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    { await _service.DeleteAsync(id); TempData["Success"] = "کلاس حذف شد."; return RedirectToAction(nameof(Index)); }
}
