using Microsoft.EntityFrameworkCore;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Domain.Interfaces;

namespace Tahila.Application.Services.Implementations;

// ─────────────────────────────────────────────────────────────────────────────
// این سرویس مدیریت داستان‌های موفقیت (عکس‌های قبل/بعد) رو انجام میده
// هم برای نمایش توی سایت، هم برای ادمین که بتونه اضافه/ویرایش/حذف کنه
// ─────────────────────────────────────────────────────────────────────────────
public class TransformationService : ITransformationService
{
    // ابزار کار با دیتابیس — همه چیز از طریق این میره
    private readonly IRepository<Transformation> _repo;

    // سازنده — ASP.NET خودش _repo رو میده بهمون (Dependency Injection)
    public TransformationService(IRepository<Transformation> repo) => _repo = repo;

    // ════════════════════════════════════════════════════════════════
    //   متدهای عمومی سایت
    // ════════════════════════════════════════════════════════════════

    // همه داستان‌های موفقیت فعال رو میده — فقط اونایی که IsActive = true هستن
    // بر اساس Order مرتب میشن (عدد کمتر = اول‌تر)
    public async Task<IEnumerable<Transformation>> GetAllActiveAsync() =>
        await _repo.Query()
            .Where(t => !t.IsDeleted && t.IsActive) // حذف‌شده و غیرفعال‌ها نیان
            .OrderBy(t => t.Order)
            .ToListAsync();

    // ════════════════════════════════════════════════════════════════
    //   متدهای پنل ادمین
    // ════════════════════════════════════════════════════════════════

    // همه داستان‌ها برای ادمین — حتی غیرفعال‌ها، جدیدترین اول
    public async Task<IEnumerable<Transformation>> GetAllAdminAsync() =>
        await _repo.Query()
            .Where(t => !t.IsDeleted)
            .OrderByDescending(t => t.CreatedAt) // جدیدترین ثبت‌شده اول میاد
            .ToListAsync();

    // پیدا کردن یه داستان خاص با ID — برای صفحه ویرایش
    public async Task<Transformation?> GetByIdAsync(int id) =>
        await _repo.Query().FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

    // اضافه کردن داستان موفقیت جدید
    public async Task CreateAsync(Transformation item)
    {
        item.CreatedAt = DateTime.Now; // تاریخ ثبت
        await _repo.AddAsync(item);
    }

    // ویرایش داستان موجود
    public async Task UpdateAsync(Transformation item)
    {
        item.UpdatedAt = DateTime.Now; // تاریخ آخرین تغییر
        await _repo.UpdateAsync(item);
    }

    // حذف نرم — داستان رو پاک نمیکنیم، فقط مخفیش میکنیم
    // اینطوری اگه اشتباهی حذف کردیم میتونیم برگردونیم
    public async Task DeleteAsync(int id)
    {
        var t = await _repo.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (t != null)
        {
            t.IsDeleted = true; // مخفی کن
            await _repo.UpdateAsync(t);
        }
    }
}
