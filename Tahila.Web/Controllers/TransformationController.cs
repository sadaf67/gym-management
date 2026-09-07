using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;

namespace Tahila.Web.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر صفحه "داستان‌های موفقیت" رو مدیریت میکنه
// فقط یه صفحه داره که عکس‌های قبل/بعد اعضا رو نشون میده
// ─────────────────────────────────────────────────────────────────────────────
public class TransformationController : Controller
{
    // سرویس داستان‌های موفقیت
    private readonly ITransformationService _service;

    // سازنده — سرویس رو از DI میگیره
    public TransformationController(ITransformationService service) => _service = service;

    // ── صفحه داستان‌های موفقیت: آدرس /transformations ──────────────────────
    [HttpGet("/transformations")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "داستان‌های موفقیت";

        // فقط داستان‌های فعال رو میگیریم (IsActive = true)
        // پیش از بردن به View، بر اساس Order مرتب شدن
        var items = await _service.GetAllActiveAsync();

        return View(items);
    }
}
