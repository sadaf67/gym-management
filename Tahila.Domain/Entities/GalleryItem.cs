namespace Tahila.Domain.Entities;

// ─────────────────────────────────────────────────────────────────────────────
// این کلاس "گالری باشگاه" رو نگه میداره
// توی گالری میتونیم عکس و ویدیو داشته باشیم
// مثلاً عکس از تجهیزات، عکس از کلاس‌ها، ویدیوی تبلیغاتی
// ─────────────────────────────────────────────────────────────────────────────
public class GalleryItem : BaseEntity
{
    // عنوان — مثلاً "سالن بدنسازی" یا "کلاس یوگا"
    public string Title { get; set; } = string.Empty;

    // مسیر فایل — آدرس عکس یا ویدیو توی سرور
    // مثلاً: "images/gallery/gym-hall.jpg"
    public string FilePath { get; set; } = string.Empty;

    // عکس یا ویدیو؟
    // false = عکس (پیشفرض)، true = ویدیو
    // اگه ویدیو باشه، از tag <video> استفاده میشه، وگرنه از <img>
    public bool IsVideo { get; set; } = false;

    // دسته‌بندی — برای فیلتر کردن گالری
    // مثلاً: "تجهیزات"، "کلاس‌ها"، "مربیان"
    public string? Category { get; set; }

    // ترتیب نمایش — عکس با عدد کمتر اول نشون داده میشه
    public int Order { get; set; } = 0;

    // فعال/غیرفعال — عکس غیرفعال توی گالری سایت نشون داده نمیشه
    public bool IsActive { get; set; } = true;
}
