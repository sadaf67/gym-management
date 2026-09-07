using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;
using Tahila.Web.Models;

namespace Tahila.Web.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر صفحه اصلی سایت رو مدیریت میکنه
// همه بخش‌هایی که توی صفحه home نشون داده میشن از اینجا لود میشن
// ─────────────────────────────────────────────────────────────────────────────
public class HomeController : Controller
{
    // هر کدوم از این سرویس‌ها داده‌های یه بخش از صفحه اصلی رو میارن
    private readonly ISliderService       _sliderService;      // اسلایدر بالای صفحه
    private readonly ICoachService        _coachService;       // کارت‌های مربیان
    private readonly IClassService        _classService;       // کلاس‌های ورزشی
    private readonly IPlanService         _planService;        // پکیج‌های عضویت
    private readonly ISiteSettingService  _siteSettingService; // تنظیمات کلی سایت (اسم، شماره، ...)

    // سازنده — همه سرویس‌ها رو از DI میگیره
    public HomeController(
        ISliderService sliderService,
        ICoachService coachService,
        IClassService classService,
        IPlanService planService,
        ISiteSettingService siteSettingService)
    {
        _sliderService      = sliderService;
        _coachService       = coachService;
        _classService       = classService;
        _planService        = planService;
        _siteSettingService = siteSettingService;
    }

    // ── صفحه اصلی سایت ──────────────────────────────────────────────────────
    public async Task<IActionResult> Index()
    {
        // یه ViewModel میسازیم و همه داده‌ها رو باهم داخلش میریزیم
        // اینطوری View فقط باید یه مدل دریافت کنه و همه چیز داخلشه
        var vm = new HomeViewModel
        {
            // اسلایدرهای فعال — معمولاً ۳ تا ۵ تصویر
            Sliders = (await _sliderService.GetActiveSliders()).ToList(),

            // حداکثر ۶ مربی نشون میدیم توی صفحه اصلی
            Coaches = (await _coachService.GetActiveCoachesAsync()).Take(6).ToList(),

            // حداکثر ۶ کلاس ورزشی
            Classes = (await _classService.GetActiveClassesAsync()).Take(6).ToList(),

            // همه پکیج‌های فعال عضویت
            Plans = (await _planService.GetActivePlansAsync()).ToList(),

            // تنظیمات کلی سایت — اسم باشگاه، شماره تماس، متن hero، ...
            Settings = await _siteSettingService.GetSettingsAsync()
        };

        return View(vm);
    }

    // ── صفحه خطا ────────────────────────────────────────────────────────────
    // وقتی یه خطای کلی توی سایت بیفته اینجا میاد
    // ResponseCache: هیچوقت این صفحه رو cache نکن چون هر بار فرق داره
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
