using Tahila.Domain.Enums;

namespace Tahila.Domain.Entities;

// ─────────────────────────────────────────────────────────────────────────────
// این کلاس "کلاس‌های ورزشی باشگاه" رو نگه میداره
// هر کلاس یه نوع ورزش هست — مثلاً: یوگا، بوکس، بدنسازی، کراسفیت
// کلاس با برنامه (Schedule) فرق داره — کلاس "چیه"، برنامه "کِیه"
// ─────────────────────────────────────────────────────────────────────────────
public class GymClass : BaseEntity
{
    // اسم کلاس — مثلاً "یوگا" یا "بوکس تایلندی"
    public string Name { get; set; } = string.Empty;

    // توضیح کلاس — این کلاس چیه و چه فوایدی داره
    public string? Description { get; set; }

    // عکس کلاس — تصویری که روی کارت این کلاس نشون داده میشه
    public string? ImagePath { get; set; }

    // کلاس آیکون — برای نشون دادن آیکون کلاس (با Font Awesome یا Bootstrap Icons)
    // مثلاً: "bi bi-bicycle" یا "fas fa-dumbbell"
    public string? IconClass { get; set; }

    // این کلاس برای کدوم جنس — مرد، زن، یا هر دو
    // از enum استفاده میکنیم: Male، Female، یا Both
    public Gender TargetGender { get; set; } = Gender.Both;

    // کدوم مربی این کلاس رو تدریس میکنه
    // null یعنی هنوز مربی بهش اختصاص داده نشده
    public int? CoachId { get; set; }

    // اطلاعات کامل مربی — EF Core این رو از جدول Coaches پر میکنه
    public Coach? Coach { get; set; }

    // ظرفیت کلاس — حداکثر چند نفر میتونن توی این کلاس باشن
    public int Capacity { get; set; } = 20;

    // ترتیب نمایش — کلاس با عدد کمتر اول نشون داده میشه
    public int Order { get; set; } = 0;

    // فعال/غیرفعال — کلاس غیرفعال توی سایت نشون داده نمیشه
    public bool IsActive { get; set; } = true;

    // برنامه هفتگی این کلاس — این کلاس چه روزهایی و چه ساعتی برگزار میشه
    // یه کلاس میتونه چند برنامه داشته باشه — مثلاً هم شنبه هم دوشنبه
    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
