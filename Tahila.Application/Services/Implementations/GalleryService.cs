using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Domain.Interfaces;

namespace Tahila.Application.Services.Implementations;

// ─────────────────────────────────────────────────────────────────────────────
// این سرویس همه کارهای "گالری عکس و ویدیو" رو مدیریت میکنه
// گالری شامل عکس‌ها و ویدیوهای باشگاهه — تجهیزات، کلاس‌ها، مربیان
// ─────────────────────────────────────────────────────────────────────────────
public class GalleryService : IGalleryService
{
    // ابزار ارتباط با جدول GalleryItems در دیتابیس
    private readonly IRepository<GalleryItem> _repo;

    public GalleryService(IRepository<GalleryItem> repo) => _repo = repo;

    // ── آیتم‌های فعال گالری برای نمایش توی سایت ──────────────────────────
    // فقط عکس‌ها و ویدیوهایی که IsActive=true هستن
    // به ترتیب Order — کوچیکتر اول نشون داده میشه
    public async Task<IEnumerable<GalleryItem>> GetActiveItemsAsync()
        => (await _repo.FindAsync(g => g.IsActive && !g.IsDeleted)).OrderBy(g => g.Order);

    // ── همه آیتم‌های گالری برای پنل ادمین ────────────────────────────────
    public async Task<IEnumerable<GalleryItem>> GetAllAsync()
        => (await _repo.FindAsync(g => !g.IsDeleted)).OrderBy(g => g.Order);

    // ── پیدا کردن یه آیتم با ID ───────────────────────────────────────────
    public async Task<GalleryItem?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    // ── اضافه کردن عکس/ویدیو جدید ────────────────────────────────────────
    public async Task CreateAsync(GalleryItem item)
    {
        item.CreatedAt = DateTime.Now;
        await _repo.AddAsync(item);
    }

    // ── ویرایش آیتم موجود ─────────────────────────────────────────────────
    public async Task UpdateAsync(GalleryItem item)
    {
        item.UpdatedAt = DateTime.Now;
        await _repo.UpdateAsync(item);
    }

    // ── حذف نرم آیتم ──────────────────────────────────────────────────────
    // فایل واقعی از سرور پاک نمیشه — فقط از دیتابیس مخفی میشه
    public async Task DeleteAsync(int id)
    {
        var g = await _repo.GetByIdAsync(id);
        if (g != null) { g.IsDeleted = true; await _repo.UpdateAsync(g); }
    }
}
