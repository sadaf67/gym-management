using Microsoft.EntityFrameworkCore;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Domain.Interfaces;

namespace Tahila.Application.Services.Implementations;

// ─────────────────────────────────────────────────────────────────────────────
// این سرویس کاملاً شبیه NutritionService هست، فقط برای مقالات تمرینی
// اگه NutritionService رو فهمیدی، اینم دقیقاً همونه
// ─────────────────────────────────────────────────────────────────────────────
public class WorkoutService : IWorkoutService
{
    private readonly IRepository<WorkoutArticle> _repo;

    public WorkoutService(IRepository<WorkoutArticle> repo) => _repo = repo;

    // همه مقالات تمرینی منتشرشده — اختیاری فیلتر دسته‌بندی
    public async Task<IEnumerable<WorkoutArticle>> GetAllAsync(string? category = null)
    {
        var q = _repo.Query().Where(a => !a.IsDeleted && a.IsPublished);
        if (!string.IsNullOrEmpty(category)) q = q.Where(a => a.Category == category);
        return await q.OrderByDescending(a => a.IsFeatured).ThenBy(a => a.Order).ToListAsync();
    }

    // پیدا کردن مقاله با اسلاگ — برای صفحه نمایش مقاله
    public async Task<WorkoutArticle?> GetBySlugAsync(string slug) =>
        await _repo.Query().FirstOrDefaultAsync(a => a.Slug == slug && !a.IsDeleted);

    // هر بار که صفحه مقاله باز میشه، این صدا زده میشه تا ViewCount بره بالا
    public async Task IncrementViewAsync(int id)
    {
        var a = await _repo.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (a != null) { a.ViewCount++; await _repo.UpdateAsync(a); }
    }

    // همه دسته‌بندی‌های موجود — مثل "تمرین قدرتی"، "کاردیو"، "تمرین خانه"
    public async Task<IEnumerable<string>> GetCategoriesAsync() =>
        await _repo.Query()
            .Where(a => !a.IsDeleted && a.IsPublished)
            .Select(a => a.Category)
            .Distinct()
            .ToListAsync();

    // مقالات ویژه — برای نمایش در صفحه اصلی یا سایدبار
    public async Task<IEnumerable<WorkoutArticle>> GetFeaturedAsync(int count = 3) =>
        await _repo.Query()
            .Where(a => !a.IsDeleted && a.IsPublished && a.IsFeatured)
            .OrderBy(a => a.Order)
            .Take(count)
            .ToListAsync();

    // مقالات مشابه — هم‌دسته ولی خودِ مقاله نباشه
    public async Task<IEnumerable<WorkoutArticle>> GetRelatedAsync(int id, string category, int count = 3) =>
        await _repo.Query()
            .Where(a => a.Id != id && !a.IsDeleted && a.IsPublished && a.Category == category)
            .OrderByDescending(a => a.ViewCount)
            .Take(count)
            .ToListAsync();

    // ── ادمین ────────────────────────────────────────────────────────

    // همه مقالات برای ادمین — حتی پیش‌نویس‌ها، جدیدترین اول
    public async Task<IEnumerable<WorkoutArticle>> GetAllAdminAsync() =>
        await _repo.Query()
            .Where(a => !a.IsDeleted)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

    // پیدا کردن مقاله با ID برای ویرایش
    public async Task<WorkoutArticle?> GetByIdAsync(int id) =>
        await _repo.Query().FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

    // ساخت مقاله جدید — تاریخ ثبت + اتوماتیک ساخت Slug از عنوان
    public async Task CreateAsync(WorkoutArticle article)
    {
        article.CreatedAt = DateTime.Now;

        // اگه slug خالیه، از عنوان مقاله میسازیم
        // مثلاً "تکنیک اسکوات" → "تکنیک-اسکوات-123456"
        if (string.IsNullOrEmpty(article.Slug))
            article.Slug = article.Title.Replace(" ", "-").Replace("‌", "-") + "-" + DateTime.Now.Ticks.ToString()[^6..];

        await _repo.AddAsync(article);
    }

    // ویرایش مقاله — فقط تاریخ آپدیت رو عوض میکنه و ذخیره میکنه
    public async Task UpdateAsync(WorkoutArticle article)
    {
        article.UpdatedAt = DateTime.Now;
        await _repo.UpdateAsync(article);
    }

    // حذف نرم — مقاله پاک نمیشه، فقط مخفی میشه
    public async Task DeleteAsync(int id)
    {
        var a = await _repo.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (a != null) { a.IsDeleted = true; await _repo.UpdateAsync(a); }
    }
}
