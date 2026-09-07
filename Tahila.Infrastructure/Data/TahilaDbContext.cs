using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tahila.Domain.Entities;

namespace Tahila.Infrastructure.Data;

// ─────────────────────────────────────────────────────────────────────────────
// این کلاس "دروازه اصلی" پروژه به دیتابیس هست
// هر چیزی که توی دیتابیس ذخیره میشه، از اینجا رد میشه
//
// IdentityDbContext: از Microsoft.AspNetCore.Identity ارث برده
// این یعنی جداول کاربران (AspNetUsers)، نقش‌ها (AspNetRoles) و ...
// خودکار توسط ASP.NET Identity مدیریت میشن
// ─────────────────────────────────────────────────────────────────────────────
public class TahilaDbContext : IdentityDbContext<ApplicationUser>
{
    // سازنده — تنظیمات اتصال به دیتابیس (connection string) رو میگیره
    public TahilaDbContext(DbContextOptions<TahilaDbContext> options) : base(options) { }

    // ════════════════════════════════════════════════════════════════
    //   DbSet‌ها — هر کدوم یه جدول در دیتابیس هستن
    //   با => Set<T>() اعلام میکنیم که این property یه DbSet هست
    // ════════════════════════════════════════════════════════════════

    // تنظیمات کلی سایت — اسم باشگاه، شماره تماس، متن hero، ...
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();

    // اسلایدرهای صفحه اصلی — عکس‌های بزرگ بالای صفحه
    public DbSet<Slider> Sliders => Set<Slider>();

    // مربیان باشگاه
    public DbSet<Coach> Coaches => Set<Coach>();

    // کلاس‌های ورزشی — یوگا، بوکس، ...
    public DbSet<GymClass> GymClasses => Set<GymClass>();

    // برنامه هفتگی — هر روز هفته چه کلاسی داره
    public DbSet<Schedule> Schedules => Set<Schedule>();

    // پکیج‌های عضویت — ماهانه، فصلی، سالانه
    public DbSet<Plan> Plans => Set<Plan>();

    // اشتراک‌های فعال کاربران — کدوم کاربر کدوم پکیج رو خریده
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    // سابقه پرداخت‌ها — تراکنش‌های مالی
    public DbSet<Payment> Payments => Set<Payment>();

    // گالری عکس‌های باشگاه
    public DbSet<GalleryItem> GalleryItems => Set<GalleryItem>();

    // تحلیل‌های بدن (BMI) — هر بار که کسی BMI محاسبه کرد
    public DbSet<BodyAnalysis> BodyAnalyses => Set<BodyAnalysis>();

    // دوره‌های آموزشی آفلاین (ویدیویی)
    public DbSet<OfflineClass> OfflineClasses => Set<OfflineClass>();

    // جلسات هر دوره آموزشی — هر دوره چند جلسه داره
    public DbSet<ClassSession> ClassSessions => Set<ClassSession>();

    // مقالات تغذیه
    public DbSet<NutritionArticle> NutritionArticles => Set<NutritionArticle>();

    // مقالات تمرین
    public DbSet<WorkoutArticle> WorkoutArticles => Set<WorkoutArticle>();

    // داستان‌های موفقیت (عکس قبل/بعد اعضا)
    public DbSet<Transformation> Transformations => Set<Transformation>();

    // ════════════════════════════════════════════════════════════════
    //   تنظیمات اضافه — وقتی دیتابیس ساخته میشه این اجرا میشه
    // ════════════════════════════════════════════════════════════════
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // اول باید base رو صدا بزنیم تا جداول Identity درست بشن
        base.OnModelCreating(builder);

        // مبلغ پرداخت رو به صورت decimal با 18 رقم و بدون اعشار ذخیره کن
        // مثلاً 500000 (پانصد هزار تومان) — نه 500000.50
        builder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasColumnType("decimal(18,0)");

        // قیمت پکیج‌ها هم همینطور
        builder.Entity<Plan>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,0)");

        // ── داده‌های اولیه (Seed Data) ─────────────────────────────────────
        // این داده‌ها وقتی دیتابیس رو اول بار میسازیم خودکار وارد میشن
        // مثل یه ردیف پیشفرض توی جدول تنظیمات
        builder.Entity<SiteSetting>().HasData(new SiteSetting
        {
            Id           = 1,
            SiteName     = "تهیلا",
            HeroTitle    = "به باشگاه تهیلا خوش آمدید",
            HeroSubTitle = "قوی‌تر، سالم‌تر، بهتر",
            AboutTitle   = "درباره ما",
            AboutText    = "باشگاه تهیلا با بهترین امکانات و مربیان مجرب آماده خدمت‌رسانی به شماست.",
            Phone        = "021-00000000",
            Address      = "تهران، ...",
            WorkingHours = "شنبه تا پنج‌شنبه: ۷ الی ۲۳",
            FooterText   = "تمامی حقوق متعلق به باشگاه تهیلا می‌باشد.",
            PrimaryColor = "#FF2D78",  // رنگ اصلی سایت (صورتی)
            MembersCount = 500,        // تعداد اعضا (برای نمایش در صفحه اصلی)
            CoachesCount = 10,
            ClassesCount = 15,
            YearsCount   = 5           // سال‌های فعالیت باشگاه
        });
    }
}
