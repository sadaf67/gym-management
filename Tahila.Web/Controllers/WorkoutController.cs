using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;

namespace Tahila.Web.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر صفحات تمرین سایت رو مدیریت میکنه (قسمت عمومی)
// دقیقاً مثل NutritionController هست فقط برای مقالات تمرینی
// ─────────────────────────────────────────────────────────────────────────────
public class WorkoutController : Controller
{
    // سرویس تمرین — همه عملیات دیتابیس از اینجا
    private readonly IWorkoutService _service;

    // سازنده — سرویس رو از DI میگیره
    public WorkoutController(IWorkoutService service) => _service = service;

    // ── لیست مقالات تمرینی: آدرس /workout ──────────────────────────────────
    // cat = دسته‌بندی اختیاری — مثلاً: /workout?cat=کاردیو
    [HttpGet("/workout")]
    public async Task<IActionResult> Index(string? cat)
    {
        ViewData["Title"] = "مرکز تمرین و ورزش";

        // مقالات (فیلترشده بر اساس دسته اگه انتخاب شده)
        ViewBag.Articles = await _service.GetAllAsync(cat);

        // ۳ مقاله ویژه برای بالای صفحه
        ViewBag.Featured = await _service.GetFeaturedAsync(3);

        // لیست دسته‌بندی‌ها برای منوی فیلتر
        ViewBag.Categories = await _service.GetCategoriesAsync();

        // دسته‌ای که الان فعاله — برای هایلایت دکمه
        ViewBag.ActiveCat = cat;

        return View();
    }

    // ── صفحه یک مقاله تمرینی: آدرس /workout/{slug} ─────────────────────────
    [HttpGet("/workout/{slug}")]
    public async Task<IActionResult> Article(string slug)
    {
        // مقاله رو با اسلاگ پیدا میکنیم
        var article = await _service.GetBySlugAsync(slug);

        // اگه وجود نداشت، خطای 404 برمیگردونیم
        if (article == null) return NotFound();

        // شمارنده بازدید رو اضافه میکنیم
        await _service.IncrementViewAsync(article.Id);

        // مقالات مشابه (هم‌دسته) برای سایدبار "بیشتر بخوانید"
        ViewBag.Related = await _service.GetRelatedAsync(article.Id, article.Category);

        // عنوان تب مرورگر = عنوان مقاله
        ViewData["Title"] = article.Title;

        return View(article);
    }
}
