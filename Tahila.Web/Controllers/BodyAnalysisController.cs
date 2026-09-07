using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Domain.Enums;
using Tahila.Infrastructure.Data;

namespace Tahila.Web.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر صفحه BMI و تحلیل بدن رو مدیریت میکنه
// مراحل: ۱) نمایش فرم  ۲) محاسبه و نمایش نتیجه  ۳) رژیم غذایی  ۴) تاریخچه
// ─────────────────────────────────────────────────────────────────────────────
public class BodyAnalysisController : Controller
{
    // سرویس تحلیل بدن — محاسبات BMI و ذخیره/بازیابی
    private readonly IBodyAnalysisService _service;

    // سرویس رژیم غذایی — ساخت برنامه غذایی بر اساس BMI
    private readonly IDietPlanService _dietService;

    // برای گرفتن اطلاعات کاربر لاگین‌کرده
    private readonly UserManager<ApplicationUser> _userManager;

    // دسترسی مستقیم به دیتابیس — برای چک کردن اشتراک VIP
    private readonly TahilaDbContext _db;

    // سازنده — همه ابزارها رو از DI میگیره
    public BodyAnalysisController(
        IBodyAnalysisService service,
        IDietPlanService dietService,
        UserManager<ApplicationUser> userManager,
        TahilaDbContext db)
    {
        _service     = service;
        _dietService = dietService;
        _userManager = userManager;
        _db          = db;
    }

    // ════════════════════════════════════════════════════════════════
    //   مرحله ۱ — نمایش فرم BMI
    // ════════════════════════════════════════════════════════════════

    // صفحه فرم خالی — GET /bmi
    [HttpGet("/bmi")]
    public IActionResult Index()
    {
        ViewData["Title"] = "محاسبه BMI و تحلیل بدن";
        // یه مدل خالی میدیم به View تا فرم مقادیر پیشفرض داشته باشه
        return View(new BodyAnalysis());
    }

    // ════════════════════════════════════════════════════════════════
    //   مرحله ۲ — دریافت فرم و محاسبه نتیجه
    // ════════════════════════════════════════════════════════════════

    // پردازش فرم BMI — POST /bmi
    [HttpPost("/bmi")]
    [ValidateAntiForgeryToken] // جلوگیری از CSRF (حمله جعل درخواست)
    public async Task<IActionResult> Index(BodyAnalysis model)
    {
        ViewData["Title"] = "نتیجه تحلیل بدن";

        // اگه مقادیر اساسی وارد نشده بود، خطا بده و دوباره فرم نشون بده
        if (model.Weight <= 0 || model.Height <= 0 || model.Age <= 0)
        {
            ModelState.AddModelError("", "لطفاً همه فیلدهای ضروری را پر کنید.");
            return View(model);
        }

        // همه محاسبات رو انجام بده (BMI، کالری، چربی، توصیه‌ها، ...)
        var result = _service.Calculate(model);

        // اگه کاربر لاگین بود، نتیجه رو توی دیتابیس ذخیره کن
        // تا بعداً توی تاریخچه بتونه ببینه
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                result.UserId = user.Id;          // نتیجه رو به این کاربر وصل کن
                await _service.SaveAsync(result); // توی دیتابیس ذخیره کن
            }
        }

        // صفحه نتیجه رو با داده‌های محاسبه‌شده نشون بده
        return View("Result", result);
    }

    // ════════════════════════════════════════════════════════════════
    //   مرحله ۳ — رژیم غذایی
    // ════════════════════════════════════════════════════════════════

    // رژیم غذایی بر اساس یه نتیجه ذخیره‌شده — GET /bmi/diet/{id}
    [HttpGet("/bmi/diet/{id:int}")]
    public async Task<IActionResult> Diet(int id)
    {
        // نتیجه BMI رو از دیتابیس پیدا میکنیم
        var bmi = await _service.GetByIdAsync(id);
        if (bmi == null) return NotFound();

        // چک میکنیم آیا کاربر اشتراک VIP داره؟
        // VIP = اشتراکی که HasDietPlan = true باشه و هنوز منقضی نشده باشه
        bool isVip = false;
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                isVip = await _db.Subscriptions
                    .Include(s => s.Plan)  // جدول Plan رو هم لود کن
                    .AnyAsync(s =>
                        s.UserId == user.Id &&
                        s.Status == SubscriptionStatus.Active &&   // اشتراک فعال باشه
                        s.EndDate > DateTime.Now &&                // منقضی نشده باشه
                        s.Plan != null &&
                        s.Plan.HasDietPlan);                       // این پکیج شامل رژیم باشه
            }
        }

        // رژیم رو بساز — VIP: کامل، رایگان: محدود
        var plan = _dietService.Generate(bmi, isVip);
        ViewData["Title"] = "رژیم غذایی شخصی";
        return View(plan);
    }

    // رژیم سریع بعد از محاسبه BMI — POST /bmi/diet-quick
    // برای کسایی که لاگین نکردن و نتیجه ذخیره نشده
    [HttpPost("/bmi/diet-quick")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DietQuick(BodyAnalysis model)
    {
        // اگه مقادیر اشتباه بود، برگرد به فرم BMI
        if (model.Weight <= 0 || model.Height <= 0 || model.Age <= 0)
            return RedirectToAction("Index");

        // دوباره محاسبه میکنیم (چون نتیجه ذخیره نشده بود)
        var result = _service.Calculate(model);

        // اگه لاگین بود، VIP بودنش رو چک کن و نتیجه رو ذخیره کن
        bool isVip = false;
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                isVip = await _db.Subscriptions
                    .Include(s => s.Plan)
                    .AnyAsync(s =>
                        s.UserId == user.Id &&
                        s.Status == SubscriptionStatus.Active &&
                        s.EndDate > DateTime.Now &&
                        s.Plan != null &&
                        s.Plan.HasDietPlan);

                result.UserId = user.Id;
                await _service.SaveAsync(result); // این بار ذخیره میکنیم
            }
        }

        var plan = _dietService.Generate(result, isVip);
        ViewData["Title"] = "رژیم غذایی شخصی";
        return View("Diet", plan); // از همون View رژیم استفاده میکنیم
    }

    // ════════════════════════════════════════════════════════════════
    //   مرحله ۴ — تاریخچه
    // ════════════════════════════════════════════════════════════════

    // تاریخچه همه محاسبات کاربر — GET /bmi/history
    [HttpGet("/bmi/history")]
    public async Task<IActionResult> History()
    {
        // اگه لاگین نیست، بفرستش صفحه لاگین
        // returnUrl = بعد از لاگین برمیگرده همینجا
        if (User.Identity?.IsAuthenticated != true)
            return RedirectToAction("Login", "Account", new { returnUrl = "/bmi/history" });

        ViewData["Title"] = "تاریخچه تحلیل بدن";
        var user    = await _userManager.GetUserAsync(User);
        var history = await _service.GetUserHistoryAsync(user!.Id);
        return View(history);
    }

    // ── حذف یه نتیجه از تاریخچه — POST /bmi/delete/{id} ─────────────────
    [HttpPost("/bmi/delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id); // حذف نرم
        TempData["Success"] = "رکورد حذف شد.";
        return RedirectToAction(nameof(History)); // برگرد تاریخچه
    }
}
