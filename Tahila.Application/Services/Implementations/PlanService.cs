using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Domain.Interfaces;

namespace Tahila.Application.Services.Implementations;

// ─────────────────────────────────────────────────────────────────────────────
// این سرویس همه کارهای "پکیج‌های عضویت" رو مدیریت میکنه
// پکیج‌ها همون اشتراک‌های ماهانه / فصلی / سالانه باشگاهن
// ─────────────────────────────────────────────────────────────────────────────
public class PlanService : IPlanService
{
    // ابزار ارتباط با جدول Plans در دیتابیس
    private readonly IRepository<Plan> _repo;

    public PlanService(IRepository<Plan> repo) => _repo = repo;

    // ── پکیج‌های فعال برای نمایش توی صفحه "قیمت‌ها" ─────────────────────
    // فقط پکیج‌هایی که IsActive=true هستن — به ترتیب Order
    public async Task<IEnumerable<Plan>> GetActivePlansAsync()
        => (await _repo.FindAsync(p => p.IsActive && !p.IsDeleted)).OrderBy(p => p.Order);

    // ── همه پکیج‌ها برای پنل ادمین ────────────────────────────────────────
    public async Task<IEnumerable<Plan>> GetAllAsync()
        => (await _repo.FindAsync(p => !p.IsDeleted)).OrderBy(p => p.Order);

    // ── پیدا کردن پکیج با ID ──────────────────────────────────────────────
    public async Task<Plan?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    // ── اضافه کردن پکیج جدید ──────────────────────────────────────────────
    public async Task CreateAsync(Plan plan)
    {
        plan.CreatedAt = DateTime.Now;
        await _repo.AddAsync(plan);
    }

    // ── ویرایش پکیج موجود ─────────────────────────────────────────────────
    public async Task UpdateAsync(Plan plan)
    {
        plan.UpdatedAt = DateTime.Now;
        await _repo.UpdateAsync(plan);
    }

    // ── حذف نرم پکیج ──────────────────────────────────────────────────────
    // پکیج از دیتابیس نمیره — فقط مخفی میشه
    // چون ممکنه کاربرانی این پکیج رو قبلاً خریده باشن
    public async Task DeleteAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item != null) { item.IsDeleted = true; await _repo.UpdateAsync(item); }
    }
}
