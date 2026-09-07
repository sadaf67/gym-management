namespace Tahila.Domain.Entities;

// ─────────────────────────────────────────────────────────────────────────────
// این کلاس یه داستان موفقیت از یه عضو باشگاه رو نگه میداره
// یعنی اون عکس‌های قبل/بعد که توی صفحه "موفقیت‌ها" نشون داده میشن
// ─────────────────────────────────────────────────────────────────────────────
public class Transformation : BaseEntity
{
    // اسم عضو باشگاه — مثل "علی رضایی" یا "سارا محمدی"
    public string MemberName { get; set; } = string.Empty;

    // عنوان یا شغل عضو — اختیاریه
    // مثل "دانشجوی مهندسی" یا "کارمند اداری"
    public string? MemberTitle { get; set; }

    // آدرس عکس قبل از شروع برنامه
    public string? BeforeImageUrl { get; set; }

    // آدرس عکس بعد از برنامه — معمولاً تغییر واضحه!
    public string? AfterImageUrl { get; set; }

    // داستان کوتاه این عضو — چند جمله که خودشون گفتن
    // مثل "بعد از 4 ماه برنامه تهیلا، دیگه نفس کم نمیارم..."
    public string? Description { get; set; }

    // چند کیلو وزن کم کرد (یا اضافه کرد) — به کیلوگرم
    public float WeightLost { get; set; }

    // مدت زمان برنامه به ماه — مثلاً 4 ماه
    public int DurationMonths { get; set; }

    // BMI اول — قبل از شروع
    public float? StartBmi { get; set; }

    // BMI آخر — بعد از اتمام برنامه
    public float? EndBmi { get; set; }

    // هدف اصلی این عضو چی بوده؟
    // "کاهش وزن" یا "افزایش حجم" یا "فرم‌دهی"
    public string Goal { get; set; } = "کاهش وزن";

    // ترتیب نمایش در صفحه — عدد کمتر = بالاتر نشون داده میشه
    public int Order { get; set; } = 0;

    // فعال هست یا نه؟ — false = توی سایت نشون داده نمیشه
    public bool IsActive { get; set; } = true;
}
