using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Enums;
using Tahila.Infrastructure.Data;

namespace Tahila.Web.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر صفحات "دوره‌های ویدیویی آفلاین" رو مدیریت میکنه
// سه صفحه داره:
//   ۱) لیست دوره‌ها (/offline-classes)
//   ۲) جزئیات یه دوره + لیست جلساتش (/offline-classes/5)
//   ۳) تماشای یه جلسه (/offline-classes/watch/10)
// برای تماشا، کاربر باید اشتراک فعال داشته باشه (جلسه اول رایگانه)
// ─────────────────────────────────────────────────────────────────────────────
public class OfflineClassController : Controller
{
    // سرویس دوره‌های آفلاین — برای گرفتن دوره‌ها و جلسات
    private readonly IOfflineClassService _service;

    // UserManager: برای پیدا کردن کاربر لاگین‌کرده
    private readonly UserManager<ApplicationUser> _userManager;

    // DbContext: مستقیم به دیتابیس وصل میشیم — برای چک کردن اشتراک
    private readonly TahilaDbContext _db;

    public OfflineClassController(IOfflineClassService service,
        UserManager<ApplicationUser> userManager, TahilaDbContext db)
    { _service = service; _userManager = userManager; _db = db; }

    // ── لیست همه دوره‌ها ─────────────────────────────────────────────────────
    // آدرس: GET /offline-classes
    [HttpGet("/offline-classes")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "کلاس‌های آفلاین";
        var classes = await _service.GetAllAsync();
        return View(classes);
    }

    // ── صفحه جزئیات یه دوره ─────────────────────────────────────────────────
    // آدرس: GET /offline-classes/5
    // id = شماره دوره
    [HttpGet("/offline-classes/{id:int}")]
    public async Task<IActionResult> Detail(int id)
    {
        // دوره رو با همه جلساتش بیار
        var cls = await _service.GetByIdWithSessionsAsync(id);
        if (cls == null) return NotFound(); // اگه دوره پیدا نشد، صفحه 404

        ViewData["Title"] = cls.Title;

        // چک کن آیا کاربر اشتراک فعال داره؟
        // ViewBag.HasAccess رو به View میدیم تا نشون بده کدوم جلسات قفله
        ViewBag.HasAccess = await CheckAccessAsync();
        return View(cls);
    }

    // ── تماشای یه جلسه ───────────────────────────────────────────────────────
    // آدرس: GET /offline-classes/watch/10
    // sessionId = شماره جلسه
    [HttpGet("/offline-classes/watch/{sessionId:int}")]
    public async Task<IActionResult> Watch(int sessionId)
    {
        // جلسه رو همراه اطلاعات دوره والدش بیار
        var session = await _service.GetSessionAsync(sessionId);
        if (session == null) return NotFound();

        // اگه جلسه رایگان نیست، باید دسترسی چک بشه
        if (!session.IsFree)
        {
            // اگه لاگین نکرده، بفرستش صفحه لاگین
            // returnUrl = بعد از لاگین برمیگرده همین صفحه
            if (User.Identity?.IsAuthenticated != true)
                return RedirectToAction("Login", "Account",
                    new { returnUrl = $"/offline-classes/watch/{sessionId}" });

            // لاگین کرده ولی اشتراک داره؟
            bool hasAccess = await CheckAccessAsync();
            if (!hasAccess)
            {
                // پیام بده و بفرستش صفحه دوره برای خرید اشتراک
                TempData["NeedSubscription"] = "برای تماشای این جلسه به اشتراک فعال نیاز دارید.";
                return RedirectToAction("Detail",
                    new { id = session.OfflineClassId });
            }
        }

        ViewData["Title"] = session.Title;
        return View(session);
    }

    // ── چک کردن اشتراک فعال ──────────────────────────────────────────────────
    // private: فقط داخل همین کنترلر استفاده میشه
    // اگه کاربر اشتراک فعال و تاریخ نگذشته داشت true برمیگردونه
    private async Task<bool> CheckAccessAsync()
    {
        // اگه لاگین نکرده، دسترسی ندارد
        if (User.Identity?.IsAuthenticated != true) return false;

        // کاربر جاری رو پیدا کن
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return false;

        // چک کن آیا این کاربر یه اشتراک داره که:
        // ۱) مال خودشه (UserId)
        // ۲) وضعیتش Active هست
        // ۳) تاریخ پایانش از الان بیشتره (هنوز منقضی نشده)
        return await _db.Subscriptions
            .AnyAsync(s => s.UserId == user.Id
                        && s.Status == SubscriptionStatus.Active
                        && s.EndDate > DateTime.Now);
    }
}
