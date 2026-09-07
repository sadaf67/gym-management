using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;

namespace Tahila.Web.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر صفحه "گالری تصاویر و ویدیوها" رو نمایش میده
// آدرس: /Gallery
// کاربر اینجا عکس‌ها و ویدیوهای باشگاه رو میبینه
// ─────────────────────────────────────────────────────────────────────────────
public class GalleryController : Controller
{
    // سرویس گالری — برای گرفتن لیست عکس‌ها و ویدیوهای فعال
    private readonly IGalleryService _galleryService;

    public GalleryController(IGalleryService galleryService) => _galleryService = galleryService;

    // ── صفحه گالری ──────────────────────────────────────────────────────────
    // همه آیتم‌های فعال گالری رو میگیره — عکس و ویدیو
    // View اینها رو به صورت grid نمایش میده
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "گالری";
        return View(await _galleryService.GetActiveItemsAsync());
    }
}
