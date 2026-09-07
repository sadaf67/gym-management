using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;

namespace Tahila.Web.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر صفحه "تماس با ما" رو مدیریت میکنه
// آدرس: /Contact
// کاربر میتونه آدرس و شماره باشگاه رو ببینه و پیام بفرسته
// ─────────────────────────────────────────────────────────────────────────────
public class ContactController : Controller
{
    // سرویس تنظیمات سایت — برای گرفتن آدرس، تلفن، ساعات کاری
    private readonly ISiteSettingService _settingService;

    public ContactController(ISiteSettingService settingService) => _settingService = settingService;

    // ── صفحه تماس با ما — GET ────────────────────────────────────────────────
    // اطلاعات تماس باشگاه رو از دیتابیس میگیره و نشون میده
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "تماس با ما";

        // تنظیمات سایت رو میگیریم — شامل: آدرس، تلفن، ساعات کاری، نقشه
        return View(await _settingService.GetSettingsAsync());
    }

    // ── ارسال فرم تماس — POST ────────────────────────────────────────────────
    // وقتی کاربر فرم رو پر میکنه و ارسال میکنه اینجا میاد
    // ValidateAntiForgeryToken: جلوی حمله‌های CSRF رو میگیره
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(string name, string phone, string message)
    {
        // فعلاً پیام فقط ثبت میشه و به کاربر تشکر میگیم
        // در آینده میشه ایمیل ارسال کرد یا توی دیتابیس ذخیره کرد
        TempData["ContactSuccess"] = "پیام شما دریافت شد. به زودی با شما تماس خواهیم گرفت.";

        // بعد از ارسال، کاربر رو به همین صفحه برمیگردونیم (تا فرم ریست بشه)
        return RedirectToAction(nameof(Index));
    }
}
