namespace Tahila.Domain.Entities;

// ─────────────────────────────────────────────────────────────────────────────
// این کلاس "دوره‌های آموزشی ویدیویی" رو نگه میداره
// یعنی دوره‌هایی که کاربر میتونه آفلاین تماشا کنه — مثل یوتیوب ورزشی
// هر دوره چند جلسه داره — مثلاً دوره "کاهش وزن" → ۱۰ جلسه ویدیو
// ─────────────────────────────────────────────────────────────────────────────
public class OfflineClass : BaseEntity
{
    // عنوان دوره — مثلاً "دوره کاهش وزن با بدنسازی"
    public string Title       { get; set; } = string.Empty;

    // توضیح دوره — این دوره برای کیه، چی یاد میگیری، ...
    public string? Description { get; set; }

    // دسته‌بندی دوره — مثلاً "بدنسازی"، "یوگا"، "کاهش وزن"
    public string Category    { get; set; } = string.Empty;

    // عکس کاور دوره — تصویر پوستر دوره که روی کارتش نشون داده میشه
    public string? ThumbnailUrl { get; set; }

    // اسم مربی — مثلاً "استاد محمدی"
    // (این یه string سادهه، نه رابطه با جدول Coach)
    public string? CoachName   { get; set; }

    // عکس مربی
    public string? CoachImage  { get; set; }

    // سطح دوره — "مبتدی"، "متوسط"، یا "پیشرفته"
    public string Level        { get; set; } = "مبتدی";

    // تعداد کل جلسات — برای نمایش روی کارت دوره
    // مثلاً: "۱۰ جلسه" — این دستی وارد میشه، از Sessions.Count نمیاد
    public int    TotalSessions { get; set; }

    // ترتیب نمایش
    public int    Order        { get; set; } = 0;

    // فعال/غیرفعال
    public bool   IsActive     { get; set; } = true;

    // لیست جلسات این دوره — هر جلسه یه ویدیوی جداگانه‌ست
    // EF Core این رو خودکار از جدول ClassSessions پر میکنه
    public ICollection<ClassSession> Sessions { get; set; } = new List<ClassSession>();
}

// ─────────────────────────────────────────────────────────────────────────────
// این کلاس "جلسه‌های هر دوره" رو نگه میداره
// هر جلسه یه ویدیو یوتیوبه با عنوان و توضیح
// مثلاً دوره "کاهش وزن" → جلسه اول: "گرم کردن بدن" → لینک ویدیو
// ─────────────────────────────────────────────────────────────────────────────
public class ClassSession : BaseEntity
{
    // ID دوره‌ای که این جلسه بهش تعلق داره
    public int    OfflineClassId  { get; set; }

    // اطلاعات کامل دوره — EF Core این رو خودکار پر میکنه
    public OfflineClass OfflineClass { get; set; } = null!;

    // عنوان جلسه — مثلاً "جلسه اول: گرم کردن بدن"
    public string Title           { get; set; } = string.Empty;

    // توضیح جلسه — این جلسه چی یاد میده
    public string? Description    { get; set; }

    // لینک ویدیو — آدرس embed یوتیوب
    // مثلاً: "https://www.youtube.com/embed/abc123"
    public string VideoUrl        { get; set; } = string.Empty;

    // عکس جلسه — thumbnail پیش‌نمایش ویدیو
    public string? ThumbnailUrl   { get; set; }

    // شماره جلسه — ۱، ۲، ۳، ...
    // برای مرتب‌سازی و نمایش "جلسه X از Y"
    public int    SessionNumber   { get; set; }

    // رایگان؟ — اگه true باشه همه میتونن ببینن، حتی بدون اشتراک
    // معمولاً جلسه اول رایگانه برای جذب کاربر
    public bool   IsFree          { get; set; } = false;

    // مدت زمان جلسه — به دقیقه
    // مثلاً: 30 = نیم ساعت
    public int    DurationMinutes { get; set; } = 30;

    // ترتیب نمایش — جلسه با عدد کمتر اول نشون داده میشه
    public int    Order           { get; set; } = 0;

    // فعال/غیرفعال
    public bool   IsActive        { get; set; } = true;
}
