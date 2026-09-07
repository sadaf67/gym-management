using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Tahila.Domain.Entities;
using Tahila.Domain.Enums;

namespace Tahila.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var db = services.GetRequiredService<TahilaDbContext>();

        // ── Roles ──────────────────────────────────────────
        string[] roles = ["Admin", "Member"];
        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

        // ── Admin user ─────────────────────────────────────
        var adminEmail = "admin@tahila.ir";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail, Email = adminEmail,
                FirstName = "مدیر", LastName = "سیستم",
                IsActive = true, EmailConfirmed = true
            };
            var r = await userManager.CreateAsync(admin, "Admin@123");
            if (r.Succeeded) await userManager.AddToRoleAsync(admin, "Admin");
        }

        // ── Sample member user ─────────────────────────────
        var memberEmail = "sara@example.com";
        if (await userManager.FindByEmailAsync(memberEmail) == null)
        {
            var member = new ApplicationUser
            {
                UserName = memberEmail, Email = memberEmail,
                FirstName = "سارا", LastName = "رضایی",
                PhoneNumber = "09121234567",
                IsActive = true, EmailConfirmed = true
            };
            var r = await userManager.CreateAsync(member, "Member@123");
            if (r.Succeeded) await userManager.AddToRoleAsync(member, "Member");
        }

        // ── Sliders ────────────────────────────────────────
        if (!db.Sliders.Any())
        {
            db.Sliders.AddRange(
                new Slider
                {
                    Title = "به باشگاه تهیلا خوش آمدید",
                    SubTitle = "قوی‌تر، سالم‌تر، بهتر — با بهترین مربیان",
                    ButtonText = "شروع کن", ButtonLink = "/plans",
                    ImagePath = "", IsActive = true, Order = 1
                },
                new Slider
                {
                    Title = "برنامه تخصصی برای شما",
                    SubTitle = "تمرینات هوازی، قدرتی و یوگا با مربیان مجرب",
                    ButtonText = "مشاهده کلاس‌ها", ButtonLink = "/classes",
                    ImagePath = "", IsActive = true, Order = 2
                },
                new Slider
                {
                    Title = "اشتراک ویژه — همین حالا",
                    SubTitle = "بهترین قیمت با بهترین امکانات. همین امروز عضو شو!",
                    ButtonText = "خرید اشتراک", ButtonLink = "/plans",
                    ImagePath = "", IsActive = true, Order = 3
                }
            );
            await db.SaveChangesAsync();
        }

        // ── Coaches ────────────────────────────────────────
        if (!db.Coaches.Any())
        {
            db.Coaches.AddRange(
                new Coach
                {
                    FullName = "علی محمدی",
                    Specialty = "بدنسازی و پاورلیفتینگ",
                    Bio = "مربی درجه ۱ فدراسیون با ۱۰ سال سابقه و ۵ مدال قهرمانی کشوری.",
                    IsActive = true, Order = 1
                },
                new Coach
                {
                    FullName = "نگار احمدی",
                    Specialty = "یوگا و پیلاتس",
                    Bio = "فارغ‌التحصیل آکادمی یوگای بین‌الملل. متخصص کاهش وزن با ۷ سال تجربه.",
                    IsActive = true, Order = 2
                },
                new Coach
                {
                    FullName = "رضا کریمی",
                    Specialty = "کراس‌فیت و HIIT",
                    Bio = "مربی CrossFit Level 2 با تخصص در تمرینات پرفشار و آمادگی عمومی.",
                    IsActive = true, Order = 3
                },
                new Coach
                {
                    FullName = "مریم صادقی",
                    Specialty = "ایروبیک و زومبا",
                    Bio = "استاد تربیت بدنی، متخصص در کلاس‌های گروهی پرانرژی.",
                    IsActive = true, Order = 4
                }
            );
            await db.SaveChangesAsync();
        }

        // ── GymClasses ─────────────────────────────────────
        if (!db.GymClasses.Any())
        {
            var coachAli    = db.Coaches.First(c => c.FullName == "علی محمدی");
            var coachNegar  = db.Coaches.First(c => c.FullName == "نگار احمدی");
            var coachReza   = db.Coaches.First(c => c.FullName == "رضا کریمی");
            var coachMaryam = db.Coaches.First(c => c.FullName == "مریم صادقی");

            db.GymClasses.AddRange(
                new GymClass
                {
                    Name = "بدنسازی پیشرفته",
                    Description = "تمرینات قدرتی حرفه‌ای با تجهیزات مدرن. مناسب سطح میانی و پیشرفته.",
                    IconClass = "fas fa-dumbbell", Capacity = 20,
                    CoachId = coachAli.Id, TargetGender = Gender.Both,
                    IsActive = true, Order = 1
                },
                new GymClass
                {
                    Name = "یوگا و مدیتیشن",
                    Description = "کلاس آرامش‌بخش یوگا همراه با تنفس‌درمانی. مناسب همه سطوح.",
                    IconClass = "fas fa-spa", Capacity = 15,
                    CoachId = coachNegar.Id, TargetGender = Gender.Both,
                    IsActive = true, Order = 2
                },
                new GymClass
                {
                    Name = "پیلاتس",
                    Description = "تقویت عضلات مرکزی و بهبود وضعیت بدنی. کمردرد را ریشه‌کن کن!",
                    IconClass = "fas fa-person-walking", Capacity = 12,
                    CoachId = coachNegar.Id, TargetGender = Gender.Both,
                    IsActive = true, Order = 3
                },
                new GymClass
                {
                    Name = "کراس‌فیت",
                    Description = "تمرین پرفشار ترکیبی برای چربی‌سوزی سریع و استقامت قلبی-عروقی.",
                    IconClass = "fas fa-fire", Capacity = 12,
                    CoachId = coachReza.Id, TargetGender = Gender.Both,
                    IsActive = true, Order = 4
                },
                new GymClass
                {
                    Name = "HIIT",
                    Description = "اینتروال پرشدت ۳۰ دقیقه‌ای. چربی‌سوزی تا ۲۴ ساعت بعد از تمرین!",
                    IconClass = "fas fa-bolt", Capacity = 15,
                    CoachId = coachReza.Id, TargetGender = Gender.Both,
                    IsActive = true, Order = 5
                },
                new GymClass
                {
                    Name = "زومبا",
                    Description = "رقص و تناسب اندام با موزیک‌های شاد. کالری‌سوزی بالا با تفریح!",
                    IconClass = "fas fa-music", Capacity = 25,
                    CoachId = coachMaryam.Id, TargetGender = Gender.Both,
                    IsActive = true, Order = 6
                }
            );
            await db.SaveChangesAsync();
        }

        // ── Plans ──────────────────────────────────────────
        if (!db.Plans.Any())
        {
            db.Plans.AddRange(
                new Plan
                {
                    Name = "پایه", Price = 450000, DurationDays = 30,
                    Features = "دسترسی سالن بدنسازی|ساعات استاندارد ۸-۲۱|۱ مشاوره رایگان",
                    Description = "پلن مناسب برای شروع سفر تناسب اندام.",
                    IsActive = true, IsPopular = false, Order = 1
                },
                new Plan
                {
                    Name = "ویژه", Price = 850000, DurationDays = 30,
                    Features = "دسترسی کامل همه سالن‌ها|ساعات آزاد ۶-۲۳|۳ کلاس گروهی|ارزیابی بدنی ماهانه",
                    Description = "محبوب‌ترین پلن باشگاه با امکانات کامل.",
                    IsActive = true, IsPopular = true, Order = 2
                },
                new Plan
                {
                    Name = "پرمیوم ۳ ماهه", Price = 2200000, DurationDays = 90,
                    Features = "همه امکانات ویژه|۱۲ جلسه مربی خصوصی|برنامه غذایی|اولویت رزرو کلاس",
                    Description = "صرفه‌جویی ۳۵٪ نسبت به پلن ماهانه.",
                    IsActive = true, IsPopular = false, Order = 3
                },
                new Plan
                {
                    Name = "سالانه VIP", Price = 7500000, DurationDays = 365,
                    Features = "دسترسی نامحدود|مربی اختصاصی|رژیم غذایی|لاکر اختصاصی|اولویت کامل",
                    Description = "بهترین سرمایه‌گذاری برای سلامتی شما.",
                    IsActive = true, IsPopular = false, Order = 4
                }
            );
            await db.SaveChangesAsync();
        }

        // ── Schedules ──────────────────────────────────────
        if (!db.Schedules.Any())
        {
            var yoga    = db.GymClasses.First(c => c.Name == "یوگا و مدیتیشن");
            var hiit    = db.GymClasses.First(c => c.Name == "HIIT");
            var zumba   = db.GymClasses.First(c => c.Name == "زومبا");
            var body    = db.GymClasses.First(c => c.Name == "بدنسازی پیشرفته");
            var pilates = db.GymClasses.First(c => c.Name == "پیلاتس");
            var cross   = db.GymClasses.First(c => c.Name == "کراس‌فیت");

            db.Schedules.AddRange(
                // شنبه
                new Schedule { DayOfWeek = DayOfWeekPersian.Saturday, ClassId = yoga.Id,    StartTime = new TimeSpan(7,  0, 0), EndTime = new TimeSpan(8,  0, 0), IsActive = true },
                new Schedule { DayOfWeek = DayOfWeekPersian.Saturday, ClassId = hiit.Id,    StartTime = new TimeSpan(9,  0, 0), EndTime = new TimeSpan(9, 30, 0), IsActive = true },
                new Schedule { DayOfWeek = DayOfWeekPersian.Saturday, ClassId = zumba.Id,   StartTime = new TimeSpan(17, 0, 0), EndTime = new TimeSpan(18, 0, 0), IsActive = true },
                new Schedule { DayOfWeek = DayOfWeekPersian.Saturday, ClassId = body.Id,    StartTime = new TimeSpan(19, 0, 0), EndTime = new TimeSpan(20,15, 0), IsActive = true },
                // یکشنبه
                new Schedule { DayOfWeek = DayOfWeekPersian.Sunday,   ClassId = pilates.Id, StartTime = new TimeSpan(8,  0, 0), EndTime = new TimeSpan(9,  0, 0), IsActive = true },
                new Schedule { DayOfWeek = DayOfWeekPersian.Sunday,   ClassId = cross.Id,   StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(10,50, 0), IsActive = true },
                new Schedule { DayOfWeek = DayOfWeekPersian.Sunday,   ClassId = zumba.Id,   StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(19, 0, 0), IsActive = true },
                // دوشنبه
                new Schedule { DayOfWeek = DayOfWeekPersian.Monday,   ClassId = yoga.Id,    StartTime = new TimeSpan(7,  0, 0), EndTime = new TimeSpan(8,  0, 0), IsActive = true },
                new Schedule { DayOfWeek = DayOfWeekPersian.Monday,   ClassId = hiit.Id,    StartTime = new TimeSpan(9,  0, 0), EndTime = new TimeSpan(9, 30, 0), IsActive = true },
                new Schedule { DayOfWeek = DayOfWeekPersian.Monday,   ClassId = body.Id,    StartTime = new TimeSpan(19, 0, 0), EndTime = new TimeSpan(20,15, 0), IsActive = true },
                // سه‌شنبه
                new Schedule { DayOfWeek = DayOfWeekPersian.Tuesday,  ClassId = pilates.Id, StartTime = new TimeSpan(8,  0, 0), EndTime = new TimeSpan(9,  0, 0), IsActive = true },
                new Schedule { DayOfWeek = DayOfWeekPersian.Tuesday,  ClassId = cross.Id,   StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(10,50, 0), IsActive = true },
                new Schedule { DayOfWeek = DayOfWeekPersian.Tuesday,  ClassId = zumba.Id,   StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(19, 0, 0), IsActive = true },
                // چهارشنبه
                new Schedule { DayOfWeek = DayOfWeekPersian.Wednesday, ClassId = yoga.Id,   StartTime = new TimeSpan(7,  0, 0), EndTime = new TimeSpan(8,  0, 0), IsActive = true },
                new Schedule { DayOfWeek = DayOfWeekPersian.Wednesday, ClassId = hiit.Id,   StartTime = new TimeSpan(9,  0, 0), EndTime = new TimeSpan(9, 30, 0), IsActive = true },
                new Schedule { DayOfWeek = DayOfWeekPersian.Wednesday, ClassId = body.Id,   StartTime = new TimeSpan(19, 0, 0), EndTime = new TimeSpan(20,15, 0), IsActive = true },
                // پنجشنبه
                new Schedule { DayOfWeek = DayOfWeekPersian.Thursday,  ClassId = pilates.Id, StartTime = new TimeSpan(8,  0, 0), EndTime = new TimeSpan(9,  0, 0), IsActive = true },
                new Schedule { DayOfWeek = DayOfWeekPersian.Thursday,  ClassId = cross.Id,   StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(10,50, 0), IsActive = true },
                new Schedule { DayOfWeek = DayOfWeekPersian.Thursday,  ClassId = zumba.Id,   StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(19, 0, 0), IsActive = true },
                // جمعه
                new Schedule { DayOfWeek = DayOfWeekPersian.Friday,   ClassId = yoga.Id,    StartTime = new TimeSpan(9,  0, 0), EndTime = new TimeSpan(10, 0, 0), IsActive = true },
                new Schedule { DayOfWeek = DayOfWeekPersian.Friday,   ClassId = cross.Id,   StartTime = new TimeSpan(10,30, 0), EndTime = new TimeSpan(11,20, 0), IsActive = true }
            );
            await db.SaveChangesAsync();
        }

        // ── Gallery ────────────────────────────────────────
        if (!db.GalleryItems.Any())
        {
            db.GalleryItems.AddRange(
                new GalleryItem { Title = "سالن بدنسازی اصلی",    FilePath = "", Category = "سالن",     IsActive = true, Order = 1 },
                new GalleryItem { Title = "سالن ایروبیک",          FilePath = "", Category = "سالن",     IsActive = true, Order = 2 },
                new GalleryItem { Title = "سالن یوگا",              FilePath = "", Category = "کلاس",     IsActive = true, Order = 3 },
                new GalleryItem { Title = "رختکن VIP",              FilePath = "", Category = "امکانات",  IsActive = true, Order = 4 },
                new GalleryItem { Title = "کلاس کراس‌فیت",          FilePath = "", Category = "کلاس",     IsActive = true, Order = 5 },
                new GalleryItem { Title = "محیط باشگاه",            FilePath = "", Category = "سالن",     IsActive = true, Order = 6 }
            );
            await db.SaveChangesAsync();
        }
    }
}
