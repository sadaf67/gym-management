using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Infrastructure.FileUpload;

namespace Tahila.Web.Areas.Admin.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر پنل ادمین مقالات تمرین رو مدیریت میکنه
// کاملاً شبیه NutritionController هست، فقط برای WorkoutArticle
// آدرس: /Admin/Workout
// ─────────────────────────────────────────────────────────────────────────────
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class WorkoutController : Controller
{
    private readonly IWorkoutService _service;
    private readonly FileUploadService _fileUpload;

    public WorkoutController(IWorkoutService service, FileUploadService fileUpload)
    {
        _service    = service;
        _fileUpload = fileUpload;
    }

    // ── لیست همه مقالات تمرینی ──────────────────────────────────────────────
    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "مدیریت مقالات تمرین";
        return View(await _service.GetAllAdminAsync());
    }

    // ── فرم مقاله جدید ──────────────────────────────────────────────────────
    public IActionResult Create()
    {
        ViewData["PageTitle"] = "مقاله جدید";
        return View(new WorkoutArticle
        {
            AuthorName  = "مربی تهیلا", // نام مربی پیشفرض
            ReadMinutes = 5,
            IsPublished = true
        });
    }

    // ── ذخیره مقاله جدید ────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WorkoutArticle model, IFormFile? thumbnailFile)
    {
        ViewData["PageTitle"] = "مقاله جدید";
        try
        {
            // اگه عکس آپلود شده، اول اون رو ذخیره کن
            // عکس‌ها توی پوشه images/workout ذخیره میشن
            if (thumbnailFile != null)
                model.ThumbnailUrl = await _fileUpload.UploadImageAsync(thumbnailFile, "images/workout");

            await _service.CreateAsync(model);
            TempData["Success"] = "مقاله با موفقیت اضافه شد.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return View(model);
        }
    }

    // ── فرم ویرایش مقاله ────────────────────────────────────────────────────
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["PageTitle"] = "ویرایش مقاله";
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    // ── ذخیره تغییرات ───────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(WorkoutArticle model, IFormFile? thumbnailFile)
    {
        ViewData["PageTitle"] = "ویرایش مقاله";
        try
        {
            // اگه عکس جدید انتخاب شده، جایگزین کن
            if (thumbnailFile != null)
                model.ThumbnailUrl = await _fileUpload.UploadImageAsync(thumbnailFile, "images/workout");

            await _service.UpdateAsync(model);
            TempData["Success"] = "مقاله با موفقیت ویرایش شد.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return View(model);
        }
    }

    // ── حذف مقاله ───────────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        TempData["Success"] = "مقاله حذف شد.";
        return RedirectToAction(nameof(Index));
    }
}
