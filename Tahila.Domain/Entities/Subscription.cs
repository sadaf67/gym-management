using Tahila.Domain.Enums;

namespace Tahila.Domain.Entities;

// ─────────────────────────────────────────────────────────────────────────────
// این کلاس "اشتراک‌های فعال" رو نگه میداره
// یعنی وقتی کاربری یه پکیج میخره، یه ردیف اینجا ثبت میشه
// مثلاً: کاربر A → پکیج سه ماهه → از ۱ فروردین تا ۳۱ خرداد
// ─────────────────────────────────────────────────────────────────────────────
public class Subscription : BaseEntity
{
    // ID کاربری که این اشتراک رو خریده
    // UserId از جدول AspNetUsers میاد (جدول کاربران ASP.NET Identity)
    public string UserId { get; set; } = string.Empty;

    // ID پکیجی که خریداری شده
    public int PlanId { get; set; }

    // اطلاعات کامل پکیج — EF Core این رو از جدول Plans پر میکنه
    // با Include() میشه این رو لود کرد
    public Plan Plan { get; set; } = null!;

    // تاریخ شروع اشتراک — از این تاریخ به بعد کاربر میتونه استفاده کنه
    public DateTime StartDate { get; set; }

    // تاریخ پایان اشتراک — بعد از این تاریخ، اشتراک منقضی میشه
    // چک میکنیم: EndDate > DateTime.Now یعنی هنوز فعاله
    public DateTime EndDate { get; set; }

    // وضعیت اشتراک:
    // Pending = در انتظار پرداخت
    // Active = فعال (پرداخت موفق)
    // Expired = منقضی شده
    // Cancelled = لغو شده
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Pending;

    // لیست پرداخت‌های مربوط به این اشتراک
    // معمولاً یه پرداخت داریم، ولی میتونه بیشتر هم باشه (در صورت خطا)
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
