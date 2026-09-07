using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Domain.Interfaces;

namespace Tahila.Application.Services.Implementations;

// ─────────────────────────────────────────────────────────────────────────────
// این سرویس همه کارهای مربوط به "اسلایدرها" رو انجام میده
// یعنی اون عکس‌های بزرگ چرخانی که بالای صفحه اصلی سایت هستن
// ─────────────────────────────────────────────────────────────────────────────
public class SliderService : ISliderService
{
    // _repo یه ابزاره که با دیتابیس کار میکنه — کلاً برای جدول Sliders
    // ASP.NET خودش این رو inject میکنه — ما فقط ازش استفاده میکنیم
    private readonly IRepository<Slider> _repo;

    // سازنده — _repo رو میگیریم و ذخیره میکنیم
    public SliderService(IRepository<Slider> repo) => _repo = repo;

    // ── اسلایدرهای فعال برای صفحه اصلی سایت ──────────────────────────────
    // فقط اسلایدرهایی که IsActive=true هستن و حذف نشدن
    // بر اساس Order (ترتیب) مرتب میشن — کوچیکتر اول
    public async Task<IEnumerable<Slider>> GetActiveSliders()
        => (await _repo.FindAsync(s => s.IsActive && !s.IsDeleted)).OrderBy(s => s.Order);

    // ── همه اسلایدرها برای پنل ادمین ─────────────────────────────────────
    // غیرفعال‌ها هم نشون داده میشن — فقط حذف‌شده‌ها نه
    public async Task<IEnumerable<Slider>> GetAllSliders()
        => (await _repo.FindAsync(s => !s.IsDeleted)).OrderBy(s => s.Order);

    // ── پیدا کردن یه اسلایدر با ID ────────────────────────────────────────
    public async Task<Slider?> GetByIdAsync(int id)
        => await _repo.GetByIdAsync(id);

    // ── اضافه کردن اسلایدر جدید ────────────────────────────────────────────
    public async Task CreateAsync(Slider slider)
    {
        // تاریخ ایجاد رو الان ثبت میکنیم
        slider.CreatedAt = DateTime.Now;
        await _repo.AddAsync(slider);
    }

    // ── ویرایش اسلایدر موجود ────────────────────────────────────────────────
    public async Task UpdateAsync(Slider slider)
    {
        // تاریخ آخرین تغییر رو به‌روز میکنیم
        slider.UpdatedAt = DateTime.Now;
        await _repo.UpdateAsync(slider);
    }

    // ── حذف نرم اسلایدر ────────────────────────────────────────────────────
    // به جای حذف واقعی، IsDeleted رو true میکنیم
    // اینطوری دیتا از دیتابیس نمیره ولی توی سایت نشون داده نمیشه
    public async Task DeleteAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item != null) { item.IsDeleted = true; await _repo.UpdateAsync(item); }
    }
}
