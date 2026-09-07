using Microsoft.EntityFrameworkCore;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Domain.Interfaces;

namespace Tahila.Application.Services.Implementations;

// ─────────────────────────────────────────────────────────────────────────────
// این سرویس "دوره‌های ویدیویی آفلاین" رو مدیریت میکنه
// دو بخش داره:
//   ۱) مدیریت دوره‌ها — مثلاً "دوره کاهش وزن"
//   ۲) مدیریت جلسات — مثلاً "جلسه اول: گرم کردن بدن"
// هر دوره چند جلسه ویدیویی داره
// ─────────────────────────────────────────────────────────────────────────────
public class OfflineClassService : IOfflineClassService
{
    // ابزار ارتباط با جدول OfflineClasses
    private readonly IRepository<OfflineClass>  _classRepo;

    // ابزار ارتباط با جدول ClassSessions
    private readonly IRepository<ClassSession>  _sessionRepo;

    // سازنده — دو تا repository میگیریم، یکی برای دوره و یکی برای جلسه
    public OfflineClassService(IRepository<OfflineClass> classRepo, IRepository<ClassSession> sessionRepo)
    {
        _classRepo   = classRepo;
        _sessionRepo = sessionRepo;
    }

    // ════════════════════════════════════════════════════════════════
    //   متدهای عمومی — برای صفحات سایت (کاربران عادی)
    // ════════════════════════════════════════════════════════════════

    // ── همه دوره‌های فعال — همراه جلسات فعالشون ──────────────────────────
    // Include(c => c.Sessions.Where(...)) = فقط جلسات فعال و غیرحذف‌شده
    // این نوع Include فیلتردار فقط توی EF Core 5+ کار میکنه
    public async Task<IEnumerable<OfflineClass>> GetAllAsync() =>
        await _classRepo.Query()
            .Where(c => !c.IsDeleted && c.IsActive)
            .Include(c => c.Sessions.Where(s => !s.IsDeleted && s.IsActive))
            .OrderBy(c => c.Order)
            .ToListAsync();

    // ── یه دوره خاص با همه جلساتش ────────────────────────────────────────
    // برای صفحه جزئیات دوره — اطلاعات کامل + جلسات به ترتیب
    public async Task<OfflineClass?> GetByIdWithSessionsAsync(int id) =>
        await _classRepo.Query()
            .Where(c => c.Id == id && !c.IsDeleted)
            .Include(c => c.Sessions.Where(s => !s.IsDeleted && s.IsActive).OrderBy(s => s.Order))
            .FirstOrDefaultAsync();

    // ── پیدا کردن یه جلسه با ID — همراه اطلاعات دوره والدش ──────────────
    // برای صفحه پخش ویدیو — باید بدونیم این جلسه از کدوم دوره‌ست
    public async Task<ClassSession?> GetSessionAsync(int sessionId) =>
        await _sessionRepo.Query()
            .Include(s => s.OfflineClass)
            .FirstOrDefaultAsync(s => s.Id == sessionId && !s.IsDeleted);

    // ════════════════════════════════════════════════════════════════
    //   متدهای ادمین — برای پنل مدیریت
    // ════════════════════════════════════════════════════════════════

    // ── همه دوره‌ها برای ادمین — شامل غیرفعال‌ها هم ─────────────────────
    public async Task<IEnumerable<OfflineClass>> GetAllAdminAsync() =>
        await _classRepo.Query()
            .Where(c => !c.IsDeleted)
            .Include(c => c.Sessions.Where(s => !s.IsDeleted))
            .OrderBy(c => c.Order)
            .ToListAsync();

    // ── پیدا کردن دوره با ID برای ادمین ──────────────────────────────────
    // جلسات به ترتیب Order مرتب میشن
    public async Task<OfflineClass?> GetByIdAsync(int id) =>
        await _classRepo.Query()
            .Where(c => c.Id == id && !c.IsDeleted)
            .Include(c => c.Sessions.Where(s => !s.IsDeleted).OrderBy(s => s.Order))
            .FirstOrDefaultAsync();

    // ── اضافه کردن دوره جدید ──────────────────────────────────────────────
    public async Task CreateAsync(OfflineClass course)
    {
        course.CreatedAt = DateTime.Now;
        await _classRepo.AddAsync(course);
    }

    // ── ویرایش دوره موجود ─────────────────────────────────────────────────
    public async Task UpdateAsync(OfflineClass course)
    {
        course.UpdatedAt = DateTime.Now;
        await _classRepo.UpdateAsync(course);
    }

    // ── حذف نرم دوره — دوره از دیتابیس نمیره فقط مخفی میشه ──────────────
    public async Task DeleteAsync(int id)
    {
        var c = await _classRepo.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (c != null) { c.IsDeleted = true; await _classRepo.UpdateAsync(c); }
    }

    // ════════════════════════════════════════════════════════════════
    //   مدیریت جلسات
    // ════════════════════════════════════════════════════════════════

    // ── اضافه کردن جلسه جدید به یه دوره ──────────────────────────────────
    public async Task CreateSessionAsync(ClassSession session)
    {
        session.CreatedAt = DateTime.Now;
        await _sessionRepo.AddAsync(session);
    }

    // ── ویرایش جلسه موجود ─────────────────────────────────────────────────
    public async Task UpdateSessionAsync(ClassSession session)
    {
        session.UpdatedAt = DateTime.Now;
        await _sessionRepo.UpdateAsync(session);
    }

    // ── حذف نرم جلسه ──────────────────────────────────────────────────────
    public async Task DeleteSessionAsync(int sessionId)
    {
        var s = await _sessionRepo.Query().FirstOrDefaultAsync(x => x.Id == sessionId);
        if (s != null) { s.IsDeleted = true; await _sessionRepo.UpdateAsync(s); }
    }
}
