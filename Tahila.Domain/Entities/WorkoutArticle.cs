namespace Tahila.Domain.Entities;

// ─────────────────────────────────────────────────────────────────────────────
// این کلاس دقیقاً مثل NutritionArticle هست، فقط برای مقالات تمرینی
// مثلاً مقاله "تکنیک صحیح اسکوات" یا "برنامه 3 روزه تمام بدن"
// ─────────────────────────────────────────────────────────────────────────────
public class WorkoutArticle : BaseEntity
{
    // عنوان مقاله تمرینی — مثل "5 اشتباه رایج در تمرین قدرتی"
    public string Title { get; set; } = string.Empty;

    // بخش URL این مقاله — خودکار از عنوان ساخته میشه
    // مثل: "5-اشتباه-رایج-در-تمرین-قدرتی-987654"
    public string Slug { get; set; } = string.Empty;

    // خلاصه مقاله که توی کارت لیست نشون داده میشه
    public string Summary { get; set; } = string.Empty;

    // متن کامل — می‌تونه HTML داشته باشه (عکس، لیست، ...)
    public string Content { get; set; } = string.Empty;

    // دسته‌بندی — مثل "تمرین قدرتی"، "کاردیو"، "یوگا"، "تمرین خانه"
    public string Category { get; set; } = string.Empty;

    // برچسب‌ها با کاما — مثل: "اسکوات,پا,قدرتی"
    public string? Tags { get; set; }

    // عکس شاخص مقاله
    public string? ThumbnailUrl { get; set; }

    // نویسنده — پیشفرض مربی باشگاه هست
    public string AuthorName { get; set; } = "مربی تهیلا";

    // زمان مطالعه به دقیقه
    public int ReadMinutes { get; set; } = 5;

    // تعداد بازدید — هر بار که کسی مقاله رو باز کنه +1 میشه
    public int ViewCount { get; set; } = 0;

    // مقاله ویژه؟ — true = اول نشون داده میشه
    public bool IsFeatured { get; set; } = false;

    // منتشر شده؟ — false = پیش‌نویسه، کاربرا نمیبیننش
    public bool IsPublished { get; set; } = true;

    // ترتیب نمایش در لیست
    public int Order { get; set; } = 0;
}
