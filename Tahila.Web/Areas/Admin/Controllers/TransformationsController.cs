using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Infrastructure.FileUpload;

namespace Tahila.Web.Areas.Admin.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر پنل ادمین داستان‌های موفقیت رو مدیریت میکنه
// هر داستان دو تا عکس داره: قبل (before) و بعد (after)
// آدرس: /Admin/Transformations
// ─────────────────────────────────────────────────────────────────────────────
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class TransformationsController : Controller
{
    private readonly ITransformationService _service;

    // سرویس آپلود — برای ذخیره عکس قبل و بعد
    private readonly FileUploadService _fileUpload;

    public TransformationsController(ITransformationService service, FileUploadService fileUpload)
    {
        _service    = service;
        _fileUpload = fileUpload;
    }

    // ── لیست همه داستان‌ها ──────────────────────────────────────────────────
    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "داستان‌های موفقیت";
        return View(await _service.GetAllAdminAsync());
    }

    // ── فرم داستان جدید ─────────────────────────────────────────────────────
    public IActionResult Create()
    {
        ViewData["PageTitle"] = "داستان جدید";
        return View(new Transformation
        {
            IsActive       = true,         // پیشفرض: نشون داده بشه
            Goal           = "کاهش وزن",  // پیشفرض: هدف کاهش وزن
            DurationMonths = 3             // پیشفرض: ۳ ماه برنامه
        });
    }

    // ── ذخیره داستان جدید ───────────────────────────────────────────────────
    // beforeFile = عکس قبل، afterFile = عکس بعد — هر دو اختیاری
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Transformation model, IFormFile? beforeFile, IFormFile? afterFile)
    {
        ViewData["PageTitle"] = "داستان جدید";
        try
        {
            // اگه عکس قبل آپلود شده، ذخیره کن
            if (beforeFile != null)
                model.BeforeImageUrl = await _fileUpload.UploadImageAsync(beforeFile, "images/transformations");

            // اگه عکس بعد آپلود شده، ذخیره کن
            if (afterFile != null)
                model.AfterImageUrl = await _fileUpload.UploadImageAsync(afterFile, "images/transformations");

            await _service.CreateAsync(model);
            TempData["Success"] = "داستان با موفقیت اضافه شد.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return View(model);
        }
    }

    // ── فرم ویرایش داستان ───────────────────────────────────────────────────
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["PageTitle"] = "ویرایش داستان";
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    // ── ذخیره تغییرات ───────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Transformation model, IFormFile? beforeFile, IFormFile? afterFile)
    {
        ViewData["PageTitle"] = "ویرایش داستان";
        try
        {
            // اگه عکس جدید قبل انتخاب شده، جایگزین قبلی کن
            if (beforeFile != null)
                model.BeforeImageUrl = await _fileUpload.UploadImageAsync(beforeFile, "images/transformations");

            // اگه عکس جدید بعد انتخاب شده، جایگزین قبلی کن
            if (afterFile != null)
                model.AfterImageUrl = await _fileUpload.UploadImageAsync(afterFile, "images/transformations");

            await _service.UpdateAsync(model);
            TempData["Success"] = "داستان با موفقیت ویرایش شد.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return View(model);
        }
    }

    // ── حذف داستان ──────────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id); // حذف نرم
        TempData["Success"] = "داستان حذف شد.";
        return RedirectToAction(nameof(Index));
    }
}
