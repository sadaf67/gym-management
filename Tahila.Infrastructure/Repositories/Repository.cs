using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Tahila.Domain.Interfaces;
using Tahila.Infrastructure.Data;

namespace Tahila.Infrastructure.Repositories;

// ─────────────────────────────────────────────────────────────────────────────
// این کلاس "ابزار ارتباط با دیتابیس" هست
// به عبارت ساده‌تر: هر کاری که با دیتابیس داریم (بخون، بنویس، حذف کن)
// از اینجا رد میشه — به جای اینکه هر سرویس مستقیم با دیتابیس کار کنه
//
// Generic (<T>) یعنی این کلاس برای هر Entity کار میکنه
// مثلاً: Repository<Coach> برای مربیان، Repository<Plan> برای پکیج‌ها
// ─────────────────────────────────────────────────────────────────────────────
public class Repository<T> : IRepository<T> where T : class
{
    // DbContext — اتصال اصلی به دیتابیس
    protected readonly TahilaDbContext _context;

    // DbSet — نمایش یه جدول توی دیتابیس
    // مثلاً اگه T=Coach، این DbSet<Coach> = جدول Coaches میشه
    protected readonly DbSet<T> _dbSet;

    // سازنده — DbContext رو میگیریم و DbSet مناسب رو ازش میگیریم
    public Repository(TahilaDbContext context)
    {
        _context = context;
        // Set<T>() میگه: جدول مربوط به Entity T رو بده
        _dbSet = context.Set<T>();
    }

    // ── پیدا کردن یه رکورد با ID ──────────────────────────────────────────
    // FindAsync توی cache EF هم میگرده — سریعتر از query مستقیم
    // ? یعنی ممکنه null برگردونه اگه پیدا نشد
    public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    // ── گرفتن همه رکوردها ─────────────────────────────────────────────────
    // SELECT * FROM [جدول] — بدون هیچ فیلتری
    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    // ── جستجو با شرط ──────────────────────────────────────────────────────
    // Expression<Func<T, bool>> یعنی یه تابع شرطی مثل: c => c.IsActive
    // مثلاً: FindAsync(c => c.IsActive && !c.IsDeleted)
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.Where(predicate).ToListAsync();

    // ── اضافه کردن رکورد جدید ─────────────────────────────────────────────
    // INSERT INTO [جدول] VALUES (...)
    // بعد از AddAsync باید SaveChangesAsync صدا بزنیم تا واقعاً ذخیره بشه
    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync(); // تغییرات رو توی دیتابیس بنویس
        return entity;
    }

    // ── به‌روزرسانی رکورد موجود ────────────────────────────────────────────
    // UPDATE [جدول] SET ... WHERE Id = ...
    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity); // EF علامت میزنه که این رکورد تغییر کرده
        await _context.SaveChangesAsync();
    }

    // ── حذف واقعی رکورد ───────────────────────────────────────────────────
    // DELETE FROM [جدول] WHERE Id = ...
    // معمولاً از این استفاده نمیکنیم — بیشتر IsDeleted = true میکنیم (حذف نرم)
    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    // ── چک کردن وجود رکورد با شرط ────────────────────────────────────────
    // SELECT EXISTS (SELECT 1 FROM ...)
    // مثلاً: ExistsAsync(u => u.Email == "test@test.com")
    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.AnyAsync(predicate);

    // ── دریافت IQueryable برای query پیچیده‌تر ────────────────────────────
    // وقتی میخوایم Include یا ThenInclude داشته باشیم از این استفاده میکنیم
    // مثلاً: _repo.Query().Include(c => c.Coach).Where(...).ToListAsync()
    public IQueryable<T> Query() => _dbSet.AsQueryable();
}
