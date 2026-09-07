using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Infrastructure.FileUpload;

namespace Tahila.Web.Areas.Admin.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر "اسلایدرهای صفحه اصلی" رو در پنل ادمین مدیریت میکنه
// آدرس: /Admin/Sliders
// ادمین میتونه عکس‌های بزرگ بالای صفحه رو اضافه، ویرایش و حذف کنه
// ─────────────────────────────────────────────────────────────────────────────
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SlidersController : Controller
{
    // سرویس اسلایدر — برای عملیات CRUD
    private readonly ISliderService _service;

    // سرویس آپلود — برای ذخیره عکس اسلاید
    private readonly FileUploadService _fileUpload;

    public SlidersController(ISliderService service, FileUploadService fileUpload)
    {
        _service = service;
        _fileUpload = fileUpload;
    }

    // ── لیست همه اسلایدرها ──────────────────────────────────────────────────
    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "مدیریت اسلایدر";
        return View(await _service.GetAllSliders());
    }

    // ── فرم اسلایدر جدید ────────────────────────────────────────────────────
    public IActionResult Create()
    {
        ViewData["PageTitle"] = "اسلایدر جدید";
        return View(new Slider()); // یه اسلایدر خالی میفرستیم
    }

    // ── ذخیره اسلایدر جدید ──────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Slider model, IFormFile? imageFile)
    {
        ViewData["PageTitle"] = "اسلایدر جدید";
        if (!ModelState.IsValid) return View(model);
        try
        {
            // اگه عکس آپلود شده، ذخیره کن — پوشه: images/sliders
            if (imageFile != null)
                model.ImagePath = await _fileUpload.UploadImageAsync(imageFile, "images/sliders");

            await _service.CreateAsync(model);
            TempData["Success"] = "اسلایدر با موفقیت اضافه شد.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; return View(model); }
    }

    // ── فرم ویرایش اسلایدر ──────────────────────────────────────────────────
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["PageTitle"] = "ویرایش اسلایدر";
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    // ── ذخیره تغییرات اسلایدر ───────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Slider model, IFormFile? imageFile)
    {
        ViewData["PageTitle"] = "ویرایش اسلایدر";
        if (!ModelState.IsValid) return View(model);
        try
        {
            // اگه عکس جدید آپلود شده، جایگزین قبلی کن
            if (imageFile != null)
                model.ImagePath = await _fileUpload.UploadImageAsync(imageFile, "images/sliders");

            await _service.UpdateAsync(model);
            TempData["Success"] = "اسلایدر با موفقیت ویرایش شد.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; return View(model); }
    }

    // ── حذف اسلایدر ─────────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id); // حذف نرم — IsDeleted = true
        TempData["Success"] = "اسلایدر حذف شد.";
        return RedirectToAction(nameof(Index));
    }
}
