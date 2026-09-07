using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Infrastructure.FileUpload;

namespace Tahila.Web.Areas.Admin.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر پنل ادمین مقالات تغذیه رو مدیریت میکنه
// [Area("Admin")] = این کنترلر توی Area ادمین هست (مسیر: /Admin/Nutrition)
// [Authorize(Roles = "Admin")] = فقط کسی که نقش Admin داره میتونه وارد بشه
// ─────────────────────────────────────────────────────────────────────────────
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class NutritionController : Controller
{
    // سرویس مقالات تغذیه
    private readonly INutritionService _service;

    // سرویس آپلود فایل — برای ذخیره عکس شاخص مقاله
    private readonly FileUploadService _fileUpload;

    // سازنده
    public NutritionController(INutritionService service, FileUploadService fileUpload)
    {
        _service    = service;
        _fileUpload = fileUpload;
    }

    // ── لیست همه مقالات — GET /Admin/Nutrition ──────────────────────────────
    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "مدیریت مقالات تغذیه";
        // همه مقالات رو بیار (حتی پیش‌نویس‌ها) و به View بده
        return View(await _service.GetAllAdminAsync());
    }

    // ── فرم مقاله جدید — GET /Admin/Nutrition/Create ────────────────────────
    public IActionResult Create()
    {
        ViewData["PageTitle"] = "مقاله جدید";
        // یه مدل با مقادیر پیشفرض میدیم تا فرم از قبل پر باشه
        return View(new NutritionArticle
        {
            AuthorName   = "دکتر تغذیه تهیلا", // نام نویسنده پیشفرض
            ReadMinutes  = 5,                    // زمان مطالعه پیشفرض: ۵ دقیقه
            IsPublished  = true                  // پیشفرض: منتشرشده
        });
    }

    // ── ذخیره مقاله جدید — POST /Admin/Nutrition/Create ────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NutritionArticle model, IFormFile? thumbnailFile)
    {
        ViewData["PageTitle"] = "مقاله جدید";
        try
        {
            // اگه ادمین یه عکس آپلود کرده، اون رو ذخیره کن و URL بگیر
            // عکس‌ها توی پوشه images/nutrition ذخیره میشن
            if (thumbnailFile != null)
                model.ThumbnailUrl = await _fileUpload.UploadImageAsync(thumbnailFile, "images/nutrition");

            // مقاله رو توی دیتابیس ذخیره کن
            await _service.CreateAsync(model);

            TempData["Success"] = "مقاله با موفقیت اضافه شد.";
            return RedirectToAction(nameof(Index)); // برگرد به لیست
        }
        catch (Exception ex)
        {
            // اگه خطایی بود (مثلاً آپلود عکس مشکل داشت)، نشون بده
            TempData["Error"] = ex.Message;
            return View(model); // دوباره فرم رو با اطلاعات وارد‌شده نشون بده
        }
    }

    // ── فرم ویرایش مقاله — GET /Admin/Nutrition/Edit/{id} ──────────────────
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["PageTitle"] = "ویرایش مقاله";
        var item = await _service.GetByIdAsync(id);

        // اگه مقاله پیدا نشد، خطای 404 بده
        if (item == null) return NotFound();

        return View(item); // مقاله رو با مقادیر پر‌شده نشون بده
    }

    // ── ذخیره تغییرات مقاله — POST /Admin/Nutrition/Edit ───────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(NutritionArticle model, IFormFile? thumbnailFile)
    {
        ViewData["PageTitle"] = "ویرایش مقاله";
        try
        {
            // اگه عکس جدید آپلود شده، جایگزین قبلی کن
            if (thumbnailFile != null)
                model.ThumbnailUrl = await _fileUpload.UploadImageAsync(thumbnailFile, "images/nutrition");

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

    // ── حذف مقاله — POST /Admin/Nutrition/Delete/{id} ──────────────────────
    // فقط POST — جلوگیری از حذف اتفاقی با یه کلیک روی لینک
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id); // حذف نرم (IsDeleted = true)
        TempData["Success"] = "مقاله حذف شد.";
        return RedirectToAction(nameof(Index));
    }
}
