using Tahila.Domain.Enums;

namespace Tahila.Domain.Entities;

// ─────────────────────────────────────────────────────────────────────────────
// این کلاس "برنامه هفتگی" رو نگه میداره
// یعنی هر کلاس چه روزی و چه ساعتی برگزار میشه
// مثلاً: کلاس یوگا → شنبه → ساعت ۸ الی ۹
// توی پنل ادمین، جدول برنامه هفتگی از اینجا ساخته میشه
// ─────────────────────────────────────────────────────────────────────────────
public class Schedule : BaseEntity
{
    // ID کلاسی که این برنامه بهش تعلق داره
    // مثلاً ClassId=5 یعنی برنامه کلاس شماره ۵
    public int ClassId { get; set; }

    // اطلاعات کامل کلاس — EF Core این رو خودکار از جدول GymClasses میخونه
    // null! یعنی ما مطمئنیم این مقدار null نیست ولی C# هنوز خبر نداره
    public GymClass GymClass { get; set; } = null!;

    // روز هفته — از یه enum استفاده میکنیم که اسم روزها فارسیه
    // مثلاً: Shanbeh، Yekshanbeh، Doshanbeh، ...
    public DayOfWeekPersian DayOfWeek { get; set; }

    // ساعت شروع کلاس — مثلاً 08:00:00
    // TimeSpan یعنی "مدت زمان" — اینجا از ظهر محاسبه میشه
    public TimeSpan StartTime { get; set; }

    // ساعت پایان کلاس — مثلاً 09:00:00
    public TimeSpan EndTime { get; set; }

    // فعال/غیرفعال — برنامه غیرفعال توی جدول هفتگی نشون داده نمیشه
    public bool IsActive { get; set; } = true;
}
