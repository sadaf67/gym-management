namespace Tahila.Domain.Entities;

// ─────────────────────────────────────────────────────────────────────────────
// این کلاس اطلاعات تحلیل بدن یه نفر رو نگه میداره
// هم ورودی‌هایی که کاربر وارد می‌کنه (وزن، قد، سن، ...)
// هم نتایجی که ما محاسبه می‌کنیم (BMI، کالری، درصد چربی، ...)
// ─────────────────────────────────────────────────────────────────────────────
public class BodyAnalysis : BaseEntity
{
    // اگه کاربر لاگین کرده باشه، ID اون رو اینجا ذخیره می‌کنیم
    // اگه مهمون (لاگین نکرده) باشه، این null هست
    public string? UserId { get; set; }

    // اسم مهمون — برای وقتی که لاگین نیست (اختیاریه)
    public string? GuestName { get; set; }

    // ════════════════ ورودی‌هایی که کاربر وارد می‌کنه ════════════════

    // وزن کاربر به کیلوگرم
    public float Weight { get; set; }

    // قد کاربر به سانتی‌متر (مثلاً 175)
    public float Height { get; set; }

    // سن کاربر به سال
    public int Age { get; set; }

    // جنسیت: true = آقا، false = خانم
    // چون فرمول‌های BMI و BMR برای مرد و زن فرق دارن
    public bool IsMale { get; set; }

    // دور کمر به سانتی‌متر — اختیاریه، اگه وارد بشه دقت محاسبه چربی بیشتر میشه
    public float? WaistCircumference { get; set; }

    // دور باسن به سانتی‌متر — برای محاسبه دقیق‌تر چربی خانم‌ها استفاده میشه
    public float? HipCircumference { get; set; }

    // دور گردن به سانتی‌متر — بخشی از فرمول Navy برای درصد چربی
    public float? NeckCircumference { get; set; }

    // سطح فعالیت روزانه:
    // 1 = کم تحرک (پشت میز نشین، ورزش ندارن)
    // 2 = متوسط (هفته‌ای 3-5 روز ورزش)
    // 3 = فعال (6-7 روز ورزش سنگین)
    // 4 = خیلی فعال (ورزشکار حرفه‌ای یا کار بدنی)
    public int ActivityLevel { get; set; } = 2; // پیشفرض: متوسط

    // ════════════════ نتایج محاسبه‌شده توسط سیستم ════════════════

    // شاخص توده بدنی — وزن تقسیم بر قد به توان 2 (متر)
    public float BMI { get; set; }

    // دسته‌بندی BMI: "نرمال"، "اضافه وزن"، "لاغری"، ...
    public string BMICategory { get; set; } = string.Empty;

    // حداقل وزن ایده‌آل برای این قد (محدوده سبک‌تر)
    public float IdealWeightMin { get; set; }

    // حداکثر وزن ایده‌آل برای این قد (محدوده سنگین‌تر)
    public float IdealWeightMax { get; set; }

    // BMR = Basal Metabolic Rate = متابولیسم پایه
    // یعنی اگه تمام روز دراز بکشی و هیچ کاری نکنی، بدنت چند کالری میسوزونه
    public float BMR { get; set; }

    // TDEE = Total Daily Energy Expenditure = کل کالری مورد نیاز روزانه
    // این BMR ضربدر ضریب فعالیت هست
    public float TDEE { get; set; }

    // درصد چربی بدن — اگه null باشه یعنی نتونستیم محاسبه کنیم
    public float? BodyFatPercentage { get; set; }

    // توده عضلانی بدن (وزن منهای چربی)
    public float? LeanBodyMass { get; set; }

    // توصیه‌های شخصی — چند جمله که با | از هم جدا شدن
    // مثلاً: "باید 5 کیلو کم کنید | روزانه 1800 کالری بخورید | ..."
    public string Recommendation { get; set; } = string.Empty;
}
