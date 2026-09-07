using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Infrastructure.FileUpload;

namespace Tahila.Web.Areas.Admin.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر "مربیان باشگاه" رو در پنل ادمین مدیریت میکنه
// آدرس: /Admin/Coaches
// ادمین میتونه مربی اضافه کنه، عکسش رو آپلود کنه، ویرایش و حذف کنه
// ─────────────────────────────────────────────────────────────────────────────
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CoachesController : Controller
{
    // سرویس مربیان — برای عملیات CRUD
    private readonly ICoachService _service;

    // سرویس آپلود — برای ذخیره عکس مربی
    private readonly FileUploadService _fileUpload;

    public CoachesController(ICoachService service, FileUploadService fileUpload)
    { _service = service; _fileUpload = fileUpload; }

    // ── لیست همه مربیان ─────────────────────────────────────────────────────
    public async Task<IActionResult> Index()
    { ViewData["PageTitle"] = "مدیریت مربیان"; return View(await _service.GetAllAsync()); }

    // ── فرم مربی جدید ───────────────────────────────────────────────────────
    public IActionResult Create()
    { ViewData["PageTitle"] = "مربی جدید"; return View(new Coach()); }

    // ── ذخیره مربی جدید ─────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Coach model, IFormFile? imageFile)
    {
        ViewData["PageTitle"] = "مربی جدید";
        try
        {
            // اگه عکس مربی آپلود شده، ذخیره کن — پوشه: images/coaches
            if (imageFile != null) model.ImagePath = await _fileUpload.UploadImageAsync(imageFile, "images/coaches");
            await _service.CreateAsync(model);
            TempData["Success"] = "مربی با موفقیت اضافه شد.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; return View(model); }
    }

    // ── فرم ویرایش مربی ─────────────────────────────────────────────────────
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["PageTitle"] = "ویرایش مربی";
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    // ── ذخیره تغییرات مربی ──────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Coach model, IFormFile? imageFile)
    {
        ViewData["PageTitle"] = "ویرایش مربی";
        try
        {
            // اگه عکس جدید انتخاب شده، جایگزین عکس قبلی کن
            if (imageFile != null) model.ImagePath = await _fileUpload.UploadImageAsync(imageFile, "images/coaches");
            await _service.UpdateAsync(model);
            TempData["Success"] = "مربی با موفقیت ویرایش شد.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; return View(model); }
    }

    // ── حذف مربی ────────────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    { await _service.DeleteAsync(id); TempData["Success"] = "مربی حذف شد."; return RedirectToAction(nameof(Index)); }
}
