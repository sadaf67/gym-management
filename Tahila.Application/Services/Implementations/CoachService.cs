using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Domain.Interfaces;

namespace Tahila.Application.Services.Implementations;

// ─────────────────────────────────────────────────────────────────────────────
// این سرویس همه کارهای مربوط به "مربیان باشگاه" رو انجام میده
// اضافه کردن مربی، ویرایش، حذف، و نمایش لیست مربیان
// ─────────────────────────────────────────────────────────────────────────────
public class CoachService : ICoachService
{
    // ابزار ارتباط با جدول Coaches در دیتابیس
    private readonly IRepository<Coach> _repo;

    // سازنده — _repo رو دریافت و ذخیره میکنیم
    public CoachService(IRepository<Coach> repo) => _repo = repo;

    // ── مربیان فعال برای نمایش توی صفحه اصلی ─────────────────────────────
    // فقط مربیانی که IsActive=true هستن — بر اساس Order مرتب میشن
    public async Task<IEnumerable<Coach>> GetActiveCoachesAsync()
        => (await _repo.FindAsync(c => c.IsActive && !c.IsDeleted)).OrderBy(c => c.Order);

    // ── همه مربیان برای پنل ادمین ─────────────────────────────────────────
    // همه رو نشون بده — حتی غیرفعال‌ها — فقط حذف‌شده‌ها نه
    public async Task<IEnumerable<Coach>> GetAllAsync()
        => (await _repo.FindAsync(c => !c.IsDeleted)).OrderBy(c => c.Order);

    // ── پیدا کردن مربی با ID ──────────────────────────────────────────────
    public async Task<Coach?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    // ── اضافه کردن مربی جدید ──────────────────────────────────────────────
    public async Task CreateAsync(Coach coach)
    {
        coach.CreatedAt = DateTime.Now;
        await _repo.AddAsync(coach);
    }

    // ── ویرایش اطلاعات مربی ───────────────────────────────────────────────
    public async Task UpdateAsync(Coach coach)
    {
        coach.UpdatedAt = DateTime.Now;
        await _repo.UpdateAsync(coach);
    }

    // ── حذف نرم مربی ──────────────────────────────────────────────────────
    // IsDeleted = true — از صفحات سایت ناپدید میشه ولی از دیتابیس نمیره
    public async Task DeleteAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item != null) { item.IsDeleted = true; await _repo.UpdateAsync(item); }
    }
}
