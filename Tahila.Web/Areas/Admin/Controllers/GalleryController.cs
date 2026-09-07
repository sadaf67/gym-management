using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Infrastructure.FileUpload;

namespace Tahila.Web.Areas.Admin.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر "گالری عکس و ویدیو" رو در پنل ادمین مدیریت میکنه
// آدرس: /Admin/Gallery
// ادمین میتونه عکس و ویدیو آپلود کنه و از گالری حذف کنه
// ─────────────────────────────────────────────────────────────────────────────
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class GalleryController : Controller
{
    // سرویس گالری — برای CRUD
    private readonly IGalleryService _service;

    // سرویس آپلود — برای ذخیره عکس یا ویدیو
    private readonly FileUploadService _fileUpload;

    public GalleryController(IGalleryService service, FileUploadService fileUpload)
    { _service = service; _fileUpload = fileUpload; }

    // ── لیست همه آیتم‌های گالری ─────────────────────────────────────────────
    public async Task<IActionResult> Index()
    { ViewData["PageTitle"] = "مدیریت گالری"; return View(await _service.GetAllAsync()); }

    // ── فرم آیتم جدید ───────────────────────────────────────────────────────
    public IActionResult Create()
    { ViewData["PageTitle"] = "آیتم جدید"; return View(new GalleryItem()); }

    // ── ذخیره آیتم جدید ─────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GalleryItem model, IFormFile? file)
    {
        ViewData["PageTitle"] = "آیتم جدید";
        try
        {
            if (file != null)
            {
                // چک کن فایل ویدیوست یا عکس — بر اساس ContentType
                // ContentType مثل "video/mp4" یا "image/jpeg"
                model.IsVideo = file.ContentType.StartsWith("video");

                // بر اساس نوع فایل، متد آپلود مناسب رو صدا میزنیم
                model.FilePath = model.IsVideo
                    ? await _fileUpload.UploadVideoAsync(file, "images/gallery")  // ویدیو
                    : await _fileUpload.UploadImageAsync(file, "images/gallery"); // عکس
            }
            await _service.CreateAsync(model);
            TempData["Success"] = "آیتم اضافه شد.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; return View(model); }
    }

    // ── حذف آیتم ────────────────────────────────────────────────────────────
    // برای گالری ویرایش نداریم — فقط حذف و اضافه
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    { await _service.DeleteAsync(id); TempData["Success"] = "آیتم حذف شد."; return RedirectToAction(nameof(Index)); }
}
