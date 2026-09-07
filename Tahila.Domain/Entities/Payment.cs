using Tahila.Domain.Enums;

namespace Tahila.Domain.Entities;

// ─────────────────────────────────────────────────────────────────────────────
// این کلاس "سابقه پرداخت‌ها" رو نگه میداره
// هر بار که کاربر پول میده (موفق یا ناموفق)، یه ردیف اینجا ثبت میشه
// مثلاً: کاربر A → ۵۰۰,۰۰۰ تومان → موفق → کد رهگیری: 123456
// ─────────────────────────────────────────────────────────────────────────────
public class Payment : BaseEntity
{
    // ID کاربری که پرداخت کرده
    public string UserId { get; set; } = string.Empty;

    // ID اشتراکی که این پرداخت برای اونه
    // یعنی این پول برای خرید کدوم پکیج بوده
    public int SubscriptionId { get; set; }

    // اطلاعات کامل اشتراک — EF Core این رو خودکار پر میکنه
    public Subscription Subscription { get; set; } = null!;

    // مبلغ پرداختی — به تومان، بدون اعشار
    // توی دیتابیس decimal(18,0) ذخیره میشه
    public decimal Amount { get; set; }

    // وضعیت پرداخت:
    // Pending = در انتظار پرداخت (کاربر هنوز نرفته بانک)
    // Success = موفق (پرداخت تایید شد)
    // Failed = ناموفق (بانک رد کرد)
    // Cancelled = لغو شده (کاربر برگشت)
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    // درگاه پرداخت — کدوم شرکت پرداخت رو مدیریت کرد
    // ZarinPal، Mellat، ...
    public PaymentGateway Gateway { get; set; } = PaymentGateway.ZarinPal;

    // کد مرجع — ZarinPal این رو موقع redirect به بانک میده
    // بعد از برگشت از بانک، با این کد میریم تایید میکنیم
    public string? Authority { get; set; }

    // کد رهگیری — بانک بعد از پرداخت موفق این رو میده
    // این کد رو به کاربر نشون میدیم تا ثبت کنه
    public string? RefId { get; set; }

    // تاریخ و ساعت پرداخت موفق — اگه null باشه یعنی هنوز پرداخت نشده
    public DateTime? PaidAt { get; set; }
}
