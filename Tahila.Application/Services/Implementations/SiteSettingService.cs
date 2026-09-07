using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Domain.Interfaces;

namespace Tahila.Application.Services.Implementations;

// ─────────────────────────────────────────────────────────────────────────────
// این سرویس "تنظیمات کلی سایت" رو مدیریت میکنه
// یعنی اطلاعاتی مثل اسم باشگاه، آدرس، رنگ سایت، شبکه‌های اجتماعی
// توی دیتابیس معمولاً فقط یه ردیف داریم — همیشه Id=1
// ─────────────────────────────────────────────────────────────────────────────
public class SiteSettingService : ISiteSettingService
{
    // ابزار ارتباط با جدول SiteSettings
    private readonly IRepository<SiteSetting> _repo;

    public SiteSettingService(IRepository<SiteSetting> repo) => _repo = repo;

    // ── خوندن تنظیمات ─────────────────────────────────────────────────────
    // همه ردیف‌ها رو بیار و اولی رو برگردون
    // اگه هیچی توی دیتابیس نبود، یه شی پیشفرض برگردون (خالی نباشیم)
    public async Task<SiteSetting> GetSettingsAsync()
    {
        var settings = (await _repo.GetAllAsync()).FirstOrDefault();
        // ?? یعنی: اگه settings null بود، یه SiteSetting جدید (خالی) بساز
        return settings ?? new SiteSetting();
    }

    // ── ذخیره تغییرات تنظیمات ─────────────────────────────────────────────
    // ادمین از پنل تغییرات رو میزنه و اینجا ذخیره میشه
    public async Task UpdateSettingsAsync(SiteSetting settings)
    {
        settings.UpdatedAt = DateTime.Now;
        await _repo.UpdateAsync(settings);
    }
}
