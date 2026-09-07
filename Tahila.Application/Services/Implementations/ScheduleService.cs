using Microsoft.EntityFrameworkCore;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Domain.Enums;
using Tahila.Domain.Interfaces;

namespace Tahila.Application.Services.Implementations;

// ─────────────────────────────────────────────────────────────────────────────
// این سرویس "برنامه هفتگی باشگاه" رو مدیریت میکنه
// یعنی هر کلاس چه روزی و چه ساعتی برگزار میشه
// مثلاً: یوگا → شنبه → ۸ الی ۹ صبح
// ─────────────────────────────────────────────────────────────────────────────
public class ScheduleService : IScheduleService
{
    // ابزار ارتباط با جدول Schedules
    private readonly IRepository<Schedule> _repo;

    public ScheduleService(IRepository<Schedule> repo) => _repo = repo;

    // ── همه برنامه‌های فعال — با اطلاعات کامل کلاس و مربی ─────────────────
    // Include(s => s.GymClass) = اطلاعات کلاس رو هم بار کن
    // ThenInclude(c => c.Coach) = اطلاعات مربی اون کلاس رو هم بار کن
    // OrderBy روز → بعد ساعت: ابتدا شنبه‌ها، بعد یکشنبه‌ها، ...
    public async Task<IEnumerable<Schedule>> GetAllWithClassAsync()
        => _repo.Query().Include(s => s.GymClass).ThenInclude(c => c.Coach)
            .Where(s => s.IsActive && !s.IsDeleted).OrderBy(s => s.DayOfWeek).ThenBy(s => s.StartTime).ToList();

    // ── برنامه‌های یه روز خاص ─────────────────────────────────────────────
    // برای فیلتر کردن جدول — مثلاً "برنامه‌های شنبه رو بده"
    // DayOfWeekPersian یه enum فارسیه: Shanbeh، Yekshanbeh، ...
    public async Task<IEnumerable<Schedule>> GetByDayAsync(DayOfWeekPersian day)
        => _repo.Query().Include(s => s.GymClass).ThenInclude(c => c.Coach)
            .Where(s => s.DayOfWeek == day && s.IsActive && !s.IsDeleted).OrderBy(s => s.StartTime).ToList();

    // ── پیدا کردن یه برنامه با ID — همراه اطلاعات کلاس ──────────────────
    public async Task<Schedule?> GetByIdAsync(int id)
        => _repo.Query().Include(s => s.GymClass).FirstOrDefault(s => s.Id == id);

    // ── اضافه کردن برنامه جدید ────────────────────────────────────────────
    public async Task CreateAsync(Schedule schedule)
    {
        schedule.CreatedAt = DateTime.Now;
        await _repo.AddAsync(schedule);
    }

    // ── ویرایش برنامه موجود ───────────────────────────────────────────────
    public async Task UpdateAsync(Schedule schedule)
    {
        schedule.UpdatedAt = DateTime.Now;
        await _repo.UpdateAsync(schedule);
    }

    // ── حذف نرم برنامه ────────────────────────────────────────────────────
    public async Task DeleteAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item != null) { item.IsDeleted = true; await _repo.UpdateAsync(item); }
    }
}
