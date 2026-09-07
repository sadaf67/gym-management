using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;

namespace Tahila.Web.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر صفحات تغذیه سایت رو مدیریت میکنه (قسمت عمومی — نه ادمین)
// دو صفحه داره: لیست مقالات و صفحه نمایش یه مقاله خاص
// ─────────────────────────────────────────────────────────────────────────────
public class NutritionController : Controller
{
    // سرویس تغذیه — همه کارهای دیتابیس از اینجا رد میشه
    private readonly INutritionService _service;

    // سازنده — سرویس رو از DI میگیره
    public NutritionController(INutritionService service) => _service = service;

    // ── صفحه لیست مقالات: آدرس /nutrition ──────────────────────────────────
    // اگه cat پاس داده بشه، فقط اون دسته رو نشون میده
    // مثلاً: /nutrition?cat=کاهش وزن
    [HttpGet("/nutrition")]
    public async Task<IActionResult> Index(string? cat)
    {
        ViewData["Title"] = "مرکز تغذیه و سلامت";

        // همه مقالات (اگه دسته انتخاب شده، فیلتر شده)
        ViewBag.Articles = await _service.GetAllAsync(cat);

        // ۳ مقاله ویژه برای نمایش در بالای صفحه (بنر ویژه)
        ViewBag.Featured = await _service.GetFeaturedAsync(3);

        // لیست همه دسته‌بندی‌ها برای منوی فیلتر
        ViewBag.Categories = await _service.GetCategoriesAsync();

        // دسته فعال الان — برای هایلایت کردن دکمه انتخاب‌شده
        ViewBag.ActiveCat = cat;

        return View();
    }

    // ── صفحه یک مقاله: آدرس /nutrition/{slug} ──────────────────────────────
    // مثلاً: /nutrition/protein-guide-123456
    [HttpGet("/nutrition/{slug}")]
    public async Task<IActionResult> Article(string slug)
    {
        // مقاله رو با اسلاگ پیدا میکنیم
        var article = await _service.GetBySlugAsync(slug);

        // اگه مقاله پیدا نشد، صفحه 404 نشون میدیم
        if (article == null) return NotFound();

        // بازدید رو یه واحد اضافه میکنیم (آمار بازدید)
        await _service.IncrementViewAsync(article.Id);

        // مقالات مشابه رو میگیریم برای نمایش در سایدبار
        ViewBag.Related = await _service.GetRelatedAsync(article.Id, article.Category);

        // عنوان صفحه = عنوان مقاله (برای تب مرورگر و سئو)
        ViewData["Title"] = article.Title;

        return View(article);
    }
}
