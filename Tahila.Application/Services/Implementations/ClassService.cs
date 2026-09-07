using Microsoft.EntityFrameworkCore;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Domain.Interfaces;

namespace Tahila.Application.Services.Implementations;

// ─────────────────────────────────────────────────────────────────────────────
// این سرویس "کلاس‌های ورزشی" رو مدیریت میکنه
// مثلاً: یوگا، بوکس، بدنسازی، کراسفیت
// کلاس با برنامه فرق داره — کلاس "چیه"، برنامه "کِی و کجاست"
// ─────────────────────────────────────────────────────────────────────────────
public class ClassService : IClassService
{
    // ابزار ارتباط با جدول GymClasses
    private readonly IRepository<GymClass> _repo;

    public ClassService(IRepository<GymClass> repo) => _repo = repo;

    // ── کلاس‌های فعال برای صفحه سایت ─────────────────────────────────────
    // Include(c => c.Coach) یعنی: اطلاعات مربی رو هم بار کن
    // بدون Include، Coach خالی (null) میمونه
    public async Task<IEnumerable<GymClass>> GetActiveClassesAsync()
        => _repo.Query().Include(c => c.Coach)
            .Where(c => c.IsActive && !c.IsDeleted).OrderBy(c => c.Order).ToList();

    // ── همه کلاس‌ها برای پنل ادمین ────────────────────────────────────────
    // غیرفعال‌ها هم هستن — همراه اطلاعات مربی
    public async Task<IEnumerable<GymClass>> GetAllAsync()
        => _repo.Query().Include(c => c.Coach)
            .Where(c => !c.IsDeleted).OrderBy(c => c.Order).ToList();

    // ── پیدا کردن کلاس با ID — همراه اطلاعات مربی ────────────────────────
    public async Task<GymClass?> GetByIdAsync(int id)
        => _repo.Query().Include(c => c.Coach).FirstOrDefault(c => c.Id == id);

    // ── اضافه کردن کلاس جدید ──────────────────────────────────────────────
    public async Task CreateAsync(GymClass gymClass)
    {
        gymClass.CreatedAt = DateTime.Now;
        await _repo.AddAsync(gymClass);
    }

    // ── ویرایش کلاس موجود ─────────────────────────────────────────────────
    public async Task UpdateAsync(GymClass gymClass)
    {
        gymClass.UpdatedAt = DateTime.Now;
        await _repo.UpdateAsync(gymClass);
    }

    // ── حذف نرم کلاس ──────────────────────────────────────────────────────
    public async Task DeleteAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item != null) { item.IsDeleted = true; await _repo.UpdateAsync(item); }
    }
}
