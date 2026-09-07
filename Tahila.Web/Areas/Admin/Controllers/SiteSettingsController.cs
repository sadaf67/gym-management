using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;
using Tahila.Infrastructure.FileUpload;

namespace Tahila.Web.Areas.Admin.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر "تنظیمات کلی سایت" رو در پنل ادمین مدیریت میکنه
// آدرس: /Admin/SiteSettings
// ادمین میتونه اسم باشگاه، آدرس، رنگ، لوگو و ... رو تغییر بده
// ─────────────────────────────────────────────────────────────────────────────
[Area("Admin")]
[Authorize(Roles = "Admin")] // فقط ادمین میتونه وارد بشه
public class SiteSettingsController : Controller
{
    // سرویس تنظیمات — برای خوندن و ذخیره تنظیمات
    private readonly ISiteSettingService _service;

    // سرویس آپلود — برای ذخیره عکس لوگو، hero، و درباره ما
    private readonly FileUploadService _fileUpload;

    public SiteSettingsController(ISiteSettingService service, FileUploadService fileUpload)
    {
        _service = service;
        _fileUpload = fileUpload;
    }

    // ── نمایش فرم تنظیمات ───────────────────────────────────────────────────
    // تنظیمات فعلی رو از دیتابیس میخونه و توی فرم پر میکنه
    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "تنظیمات سایت";
        var settings = await _service.GetSettingsAsync();
        return View(settings);
    }

    // ── ذخیره تغییرات تنظیمات ───────────────────────────────────────────────
    // وقتی ادمین فرم رو submit میکنه اینجا میاد
    // سه تا عکس اختیاری هم میتونه آپلود کنه
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(Domain.Entities.SiteSetting model,
        IFormFile? logoFile, IFormFile? heroImageFile, IFormFile? aboutImageFile)
    {
        ViewData["PageTitle"] = "تنظیمات سایت";

        // اگه فرم اشتباه پر شده، دوباره همون صفحه رو نشون بده
        if (!ModelState.IsValid) return View(model);

        try
        {
            // اگه لوگوی جدید آپلود شده، ذخیره کن
            if (logoFile != null)
                model.Logo = await _fileUpload.UploadImageAsync(logoFile, "images/site");

            // اگه عکس hero جدید آپلود شده، ذخیره کن
            if (heroImageFile != null)
                model.HeroImage = await _fileUpload.UploadImageAsync(heroImageFile, "images/site");

            // اگه عکس درباره‌ما جدید آپلود شده، ذخیره کن
            if (aboutImageFile != null)
                model.AboutImage = await _fileUpload.UploadImageAsync(aboutImageFile, "images/site");

            await _service.UpdateSettingsAsync(model);
            TempData["Success"] = "تنظیمات با موفقیت ذخیره شد.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        // بعد از ذخیره، به همین صفحه برمیگردیم
        return RedirectToAction(nameof(Index));
    }
}
