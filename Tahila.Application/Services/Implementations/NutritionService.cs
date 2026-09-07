using Microsoft.EntityFrameworkCore;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Domain.Interfaces;

namespace Tahila.Application.Services.Implementations;

// ─────────────────────────────────────────────────────────────────────────────
// این سرویس همه کارهای مربوط به مقالات تغذیه رو انجام میده
// هم برای سایت عمومی (خواندن مقالات) هم برای پنل ادمین (اضافه/ویرایش/حذف)
// ─────────────────────────────────────────────────────────────────────────────
public class NutritionService : INutritionService
{
    // ابزار ارتباط با دیتابیس
    private readonly IRepository<NutritionArticle> _repo;

    // سازنده — _repo رو از DI (تزریق وابستگی) میگیره
    public NutritionService(IRepository<NutritionArticle> repo) => _repo = repo;

    // ════════════════════════════════════════════════════════════════
    //   متدهای عمومی سایت
    // ════════════════════════════════════════════════════════════════

    // همه مقالات منتشر‌شده رو برمیگردونه
    // اگه category پاس بدیم، فقط اون دسته رو برمیگردونه
    // مقالات ویژه (IsFeatured) اول میان، بعد بر اساس Order مرتب میشن
    public async Task<IEnumerable<NutritionArticle>> GetAllAsync(string? category = null)
    {
        // شروع با query پایه: نه حذف‌شده، نه پیش‌نویس
        var q = _repo.Query().Where(a => !a.IsDeleted && a.IsPublished);

        // اگه دسته‌بندی خواستن، فیلتر اضافه میکنیم
        if (!string.IsNullOrEmpty(category))
            q = q.Where(a => a.Category == category);

        // اول ویژه‌ها، بعد بر اساس Order (کوچک‌تر = بالاتر)
        return await q.OrderByDescending(a => a.IsFeatured).ThenBy(a => a.Order).ToListAsync();
    }

    // مقاله رو با اسلاگ (بخش URL) پیدا میکنه
    // مثلاً: /nutrition/protein-guide-123456 → اسلاگ = "protein-guide-123456"
    public async Task<NutritionArticle?> GetBySlugAsync(string slug) =>
        await _repo.Query().FirstOrDefaultAsync(a => a.Slug == slug && !a.IsDeleted);

    // هر بار که کسی یه مقاله رو باز کنه، ViewCount رو یه واحد اضافه میکنه
    public async Task IncrementViewAsync(int id)
    {
        var a = await _repo.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (a != null) { a.ViewCount++; await _repo.UpdateAsync(a); }
    }

    // لیست همه دسته‌بندی‌های موجود رو میده — برای منو فیلتر استفاده میشه
    public async Task<IEnumerable<string>> GetCategoriesAsync() =>
        await _repo.Query()
            .Where(a => !a.IsDeleted && a.IsPublished)
            .Select(a => a.Category)
            .Distinct()        // هر دسته فقط یه بار بیاد
            .ToListAsync();

    // مقالات ویژه رو برمیگردونه — پیشفرض 3 تا
    // برای نمایش در صفحه اصلی یا سایدبار
    public async Task<IEnumerable<NutritionArticle>> GetFeaturedAsync(int count = 3) =>
        await _repo.Query()
            .Where(a => !a.IsDeleted && a.IsPublished && a.IsFeatured)
            .OrderBy(a => a.Order)
            .Take(count)       // فقط count تا بده، بیشتر نمیخوایم
            .ToListAsync();

    // مقالات مرتبط — مقالاتی که هم‌دسته‌بندی هستن ولی خودِ مقاله نباشن
    // برای "مقالات مشابه" توی صفحه مقاله استفاده میشه
    public async Task<IEnumerable<NutritionArticle>> GetRelatedAsync(int id, string category, int count = 3) =>
        await _repo.Query()
            .Where(a => a.Id != id && !a.IsDeleted && a.IsPublished && a.Category == category)
            .OrderByDescending(a => a.ViewCount)  // پربازدیدترین‌ها رو اول بده
            .Take(count)
            .ToListAsync();

    // ════════════════════════════════════════════════════════════════
    //   متدهای پنل ادمین
    // ════════════════════════════════════════════════════════════════

    // همه مقالات رو برمیگردونه — حتی پیش‌نویس‌ها (فقط حذف‌شده‌ها رو نمیاره)
    // ادمین باید همه رو ببینه
    public async Task<IEnumerable<NutritionArticle>> GetAllAdminAsync() =>
        await _repo.Query()
            .Where(a => !a.IsDeleted)
            .OrderByDescending(a => a.CreatedAt)  // جدیدترین اول
            .ToListAsync();

    // پیدا کردن مقاله با ID — برای صفحه ویرایش
    public async Task<NutritionArticle?> GetByIdAsync(int id) =>
        await _repo.Query().FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

    // ساخت مقاله جدید
    public async Task CreateAsync(NutritionArticle article)
    {
        // تاریخ ایجاد رو ثبت میکنیم
        article.CreatedAt = DateTime.Now;

        // اگه ادمین اسلاگ وارد نکرده، خودمون از عنوان میسازیم
        if (string.IsNullOrEmpty(article.Slug))
            article.Slug = GenerateSlug(article.Title);

        await _repo.AddAsync(article);
    }

    // ویرایش مقاله موجود
    public async Task UpdateAsync(NutritionArticle article)
    {
        // تاریخ آخرین ویرایش رو به‌روز میکنیم
        article.UpdatedAt = DateTime.Now;

        // اگه اسلاگ خالیه (نباید باشه ولی احتیاط)، بسازیم
        if (string.IsNullOrEmpty(article.Slug))
            article.Slug = GenerateSlug(article.Title);

        await _repo.UpdateAsync(article);
    }

    // حذف نرم مقاله — از دیتابیس پاک نمیشه، فقط IsDeleted = true میشه
    public async Task DeleteAsync(int id)
    {
        var a = await _repo.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (a != null) { a.IsDeleted = true; await _repo.UpdateAsync(a); }
    }

    // ════════════════════════════════════════════════════════════════
    //   تابع کمکی: ساخت اسلاگ از عنوان
    // ════════════════════════════════════════════════════════════════
    private static string GenerateSlug(string title)
    {
        var slug = title.ToLower()
            .Replace(" ", "-")    // فاصله → خط تیره
            .Replace("‌", "-")   // نیم‌فاصله (ZWNJ) → خط تیره
            .Replace("/", "-");   // اسلش → خط تیره

        // 6 رقم آخر از Ticks (زمان سیستم) رو اضافه میکنیم تا slug یکتا بشه
        // اگه دو مقاله اسم یکسان داشتن، slug‌هاشون با هم فرق داشته باشن
        return slug + "-" + DateTime.Now.Ticks.ToString()[^6..];
    }
}
