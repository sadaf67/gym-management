using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Domain.Interfaces;

namespace Tahila.Application.Services.Implementations;

// ─────────────────────────────────────────────────────────────────────────────
// این سرویس همه منطق مربوط به تحلیل بدن رو داخل خودش داره
// یعنی هم محاسبات (BMI، کالری، ...) هم ذخیره توی دیتابیس
// ─────────────────────────────────────────────────────────────────────────────
public class BodyAnalysisService : IBodyAnalysisService
{
    // این _repo یه ابزار برای کار با دیتابیسه
    // از طریق اون می‌تونیم رکورد اضافه، ویرایش، حذف و پیدا کنیم
    private readonly IRepository<BodyAnalysis> _repo;

    // این تابع وقتی که برنامه راه میفته صدا زده میشه و _repo رو میگیره
    // ASP.NET خودش این رو inject میکنه، ما نگران نیستیم
    public BodyAnalysisService(IRepository<BodyAnalysis> repo) => _repo = repo;

    // ════════════════════════════════════════════════════════════════
    //   محاسبه اصلی — همه فرمول‌ها اینجان
    // ════════════════════════════════════════════════════════════════
    public BodyAnalysis Calculate(BodyAnalysis input)
    {
        // قد رو از سانتی‌متر به متر تبدیل میکنیم چون فرمول BMI به متر نیازه
        // مثلاً 175 سانت میشه 1.75 متر
        float heightM = input.Height / 100f;

        // ── محاسبه BMI ──────────────────────────────────────────────
        // فرمول BMI: وزن (کیلو) تقسیم بر قد (متر) به توان 2
        // مثلاً: 70 / (1.75 × 1.75) = 22.8
        input.BMI = MathF.Round(input.Weight / (heightM * heightM), 1);

        // حالا با عدد BMI میفهمیم چه وضعیتی داره
        input.BMICategory = GetBMICategory(input.BMI);

        // ── وزن ایده‌آل (فرمول Devine) ─────────────────────────────
        // این فرمول یه وزن ایده‌آل بر اساس قد حساب میکنه
        // مرد از 50 کیلو، زن از 45.5 کیلو شروع میکنه
        float baseIdeal = input.IsMale ? 50f : 45.5f;
        float perCm     = 2.3f; // به ازای هر اینچ بالای 5 فوت (152.4cm)، 2.3 کیلو اضافه میشه

        // چند سانت بالای 152.4 سانت داریم؟
        float cmOver152 = MathF.Max(0, input.Height - 152.4f);

        // وزن ایده‌آل وسط محدوده
        float idealMid = baseIdeal + perCm * (cmOver152 / 2.54f);

        // 10% پایین‌تر و بالاتر رو به عنوان محدوده ایده‌آل نشون میدیم
        input.IdealWeightMin = MathF.Round(idealMid * 0.9f, 1);
        input.IdealWeightMax = MathF.Round(idealMid * 1.1f, 1);

        // ── BMR — متابولیسم پایه (فرمول Mifflin-St Jeor) ───────────
        // این عدد میگه بدنت در حالت استراحت کامل چند کالری میسوزونه
        // فرمول مرد:  10×وزن + 6.25×قد − 5×سن + 5
        // فرمول زن:   10×وزن + 6.25×قد − 5×سن − 161
        float bmr = input.IsMale
            ? 10 * input.Weight + 6.25f * input.Height - 5 * input.Age + 5
            : 10 * input.Weight + 6.25f * input.Height - 5 * input.Age - 161;
        input.BMR = MathF.Round(bmr, 0);

        // ── TDEE — کالری واقعی روزانه ───────────────────────────────
        // BMR رو ضربدر ضریب فعالیت میکنیم
        // هر چی بیشتر ورزش کنی، ضریب بیشتره پس کالری بیشتری نیاز داری
        float activityFactor = input.ActivityLevel switch
        {
            1 => 1.2f,    // کم تحرک — پشت میز نشین
            2 => 1.375f,  // متوسط — هفته‌ای 3-5 روز ورزش
            3 => 1.55f,   // فعال — هفته‌ای 6-7 روز ورزش سنگین
            4 => 1.725f,  // خیلی فعال — ورزشکار حرفه‌ای
            _ => 1.375f   // پیشفرض: متوسط
        };
        input.TDEE = MathF.Round(bmr * activityFactor, 0);

        // ── درصد چربی بدن ───────────────────────────────────────────
        // اگه کاربر دور کمر و گردن رو وارد کرده، از فرمول دقیق‌تر Navy استفاده میکنیم
        if (input.WaistCircumference.HasValue && input.NeckCircumference.HasValue)
        {
            float bf;
            if (input.IsMale)
            {
                // فرمول Navy برای مردا: از لگاریتم (کمر - گردن) و قد استفاده میکنه
                bf = 495f / (1.0324f - 0.19077f * MathF.Log10(input.WaistCircumference.Value - input.NeckCircumference.Value)
                     + 0.15456f * MathF.Log10(input.Height)) - 450f;
            }
            else
            {
                // فرمول Navy برای خانم‌ها: باسن هم اضافه میشه
                // اگه باسن وارد نشده، تخمین میزنیم (1.1 برابر کمر)
                float hip = input.HipCircumference ?? (input.WaistCircumference.Value * 1.1f);
                bf = 495f / (1.29579f - 0.35004f * MathF.Log10(input.WaistCircumference.Value + hip - input.NeckCircumference.Value)
                     + 0.22100f * MathF.Log10(input.Height)) - 450f;
            }
            // عدد رو بین 3 تا 60 نگه میداریم — اگه فرمول جواب عجیب داد، برش میزنیم
            input.BodyFatPercentage = MathF.Round(MathF.Max(3, MathF.Min(60, bf)), 1);
        }
        else
        {
            // اگه اندازه‌ها نبودن، از BMI تخمین میزنیم (کم‌دقت‌تره ولی خب چاره‌ای نیست)
            float bfEst = input.IsMale
                ? 1.20f * input.BMI + 0.23f * input.Age - 16.2f
                : 1.20f * input.BMI + 0.23f * input.Age - 5.4f;
            input.BodyFatPercentage = MathF.Round(MathF.Max(3, MathF.Min(60, bfEst)), 1);
        }

        // توده عضلانی = وزن کل منهای وزن چربی
        // مثلاً 70 کیلو با 20% چربی → عضله = 70 × (1 - 0.20) = 56 کیلو
        input.LeanBodyMass = MathF.Round(input.Weight * (1 - input.BodyFatPercentage!.Value / 100f), 1);

        // توصیه‌های شخصی رو می‌سازیم و ذخیره میکنیم
        input.Recommendation = BuildRecommendation(input);

        return input;
    }

    // ════════════════════════════════════════════════════════════════
    //   عملیات دیتابیس (CRUD)
    // ════════════════════════════════════════════════════════════════

    // نتیجه رو توی دیتابیس ذخیره میکنه — فقط وقتی کاربر لاگین باشه صدا زده میشه
    public async Task<BodyAnalysis> SaveAsync(BodyAnalysis analysis)
    {
        analysis.CreatedAt = DateTime.Now; // تاریخ ثبت
        return await _repo.AddAsync(analysis);
    }

    // تاریخچه تمام آنالیزهای یه کاربر — از جدیدترین به قدیمی‌ترین
    public async Task<IEnumerable<BodyAnalysis>> GetUserHistoryAsync(string userId)
        => (await _repo.FindAsync(b => b.UserId == userId && !b.IsDeleted))
            .OrderByDescending(b => b.CreatedAt);

    // پیدا کردن یه رکورد خاص با ID اون
    public async Task<BodyAnalysis?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    // حذف نرم — رکورد رو پاک نمیکنیم، فقط IsDeleted رو true میکنیم
    public async Task DeleteAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item != null) { item.IsDeleted = true; await _repo.UpdateAsync(item); }
    }

    // ════════════════════════════════════════════════════════════════
    //   توابع کمکی (Helper)
    // ════════════════════════════════════════════════════════════════

    // با عدد BMI میگه وضعیت چیه — از switch expression استفاده کردیم (خیلی تمیزه)
    private static string GetBMICategory(float bmi) => bmi switch
    {
        < 16.0f => "لاغری شدید",
        < 17.0f => "لاغری متوسط",
        < 18.5f => "کمی کمتر از نرمال",
        < 25.0f => "نرمال و سالم",      // محدوده خوب
        < 30.0f => "اضافه وزن",
        < 35.0f => "چاقی درجه ۱",
        < 40.0f => "چاقی درجه ۲",
        _       => "چاقی شدید درجه ۳"  // _ یعنی "هر چیز دیگه‌ای"
    };

    // توصیه‌های شخصی رو میسازه — بر اساس BMI و درصد چربی
    private static string BuildRecommendation(BodyAnalysis a)
    {
        // یه لیست خالی برای جمع کردن توصیه‌ها
        var parts = new List<string>();

        // ── بر اساس BMI توصیه میکنیم ────────────────────────────────
        if (a.BMI < 18.5f)
        {
            // زیر وزن طبیعی
            parts.Add("وزن شما کمتر از حد نرمال است. توصیه می‌شود رژیم پرکالری با پروتئین بالا داشته باشید.");
            parts.Add($"هدف: رسیدن به وزن {a.IdealWeightMin}–{a.IdealWeightMax} کیلوگرم.");
            parts.Add("تمرین: تمرکز بر تمرینات قدرتی برای عضله‌سازی.");
        }
        else if (a.BMI < 25f)
        {
            // وضعیت ایده‌آل
            parts.Add("🎉 وزن شما در محدوده ایده‌آل است!");
            parts.Add("ادامه فعالیت بدنی منظم و تغذیه متعادل را حفظ کنید.");
        }
        else if (a.BMI < 30f)
        {
            // اضافه وزن — باید کم کنه
            float tolose = MathF.Round(a.Weight - a.IdealWeightMax, 1);
            parts.Add($"اضافه وزن دارید. باید حدود {tolose} کیلوگرم کاهش وزن داشته باشید.");
            parts.Add("توصیه: کاهش ۵۰۰ کالری روزانه از رژیم غذایی.");
            parts.Add("تمرین: ترکیب کاردیو (۳ روز) و وزنه (۳ روز) در هفته.");
        }
        else
        {
            // چاقی — جدی‌تره
            float tolose = MathF.Round(a.Weight - a.IdealWeightMax, 1);
            parts.Add($"⚠️ چاقی تشخیص داده شد. کاهش {tolose} کیلوگرم ضروری است.");
            parts.Add("حتماً با پزشک متخصص مشورت کنید.");
            parts.Add("تمرین: شروع با پیاده‌روی روزانه ۳۰ دقیقه، کم‌فشار و مستمر.");
        }

        // ── بر اساس درصد چربی توصیه اضافه میکنیم ───────────────────
        if (a.BodyFatPercentage.HasValue)
        {
            // آستانه چربی بالا: مرد بالای 25%، زن بالای 35%
            bool highFat = a.IsMale ? a.BodyFatPercentage > 25 : a.BodyFatPercentage > 35;
            // آستانه چربی خیلی پایین: مرد زیر 6%، زن زیر 14%
            bool lowFat  = a.IsMale ? a.BodyFatPercentage < 6  : a.BodyFatPercentage < 14;

            if (highFat)
                parts.Add($"درصد چربی بدن ({a.BodyFatPercentage}%) بالاست. تمرینات HIIT و کاردیو توصیه می‌شود.");
            else if (lowFat)
                parts.Add($"درصد چربی بدن ({a.BodyFatPercentage}%) خیلی پایین است. مراقب سلامتی باشید.");
        }

        // همیشه کالری‌ها رو هم اضافه میکنیم
        parts.Add($"کالری پایه (BMR): {a.BMR} کیلوکالری در روز.");
        parts.Add($"کالری مورد نیاز روزانه (TDEE): {a.TDEE} کیلوکالری.");

        // همه جمله‌ها رو با | به هم وصل میکنیم — بعداً توی View جدا جداشون میکنیم
        return string.Join(" | ", parts);
    }
}
