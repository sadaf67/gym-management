using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tahila.Infrastructure.Data;
using Tahila.Web.Models;

namespace Tahila.Web.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر همه کارهای مربوط به حساب کاربری رو مدیریت میکنه
// ورود، ثبت‌نام، خروج — همه اینجاست
// ─────────────────────────────────────────────────────────────────────────────
public class AccountController : Controller
{
    // UserManager: ابزار ASP.NET Identity برای ساخت و مدیریت کاربران
    // مثلاً: ساخت کاربر، تغییر رمز، چک کردن نقش، ...
    private readonly UserManager<ApplicationUser> _userManager;

    // SignInManager: ابزار ASP.NET Identity برای ورود و خروج
    // مثلاً: بررسی رمز، ست کردن کوکی لاگین، خروج از حساب
    private readonly SignInManager<ApplicationUser> _signInManager;

    // سازنده — هر دو ابزار رو از DI میگیره
    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // ════════════════════════════════════════════════════════════════
    //   ورود به سیستم
    // ════════════════════════════════════════════════════════════════

    // نمایش فرم لاگین — GET
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["Title"] = "ورود";

        // اگه کاربر از قبل لاگین هست، نیازی نیست دوباره لاگین کنه
        // مستقیم میفرستیمش به داشبورد عضو
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Dashboard", new { area = "Member" });

        // returnUrl = آدرسی که بعد از لاگین باید بره
        // مثلاً اگه رفت /bmi/history و لاگین نبود، بعد لاگین برگرده همونجا
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    // پردازش فرم لاگین — POST (وقتی کاربر دکمه ورود رو میزنه)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        ViewData["Title"] = "ورود";

        // اگه فرم درست پر نشده (مثلاً ایمیل خالیه)، دوباره فرم رو نشون بده
        if (!ModelState.IsValid) return View(model);

        // چک کن ایمیل + رمز درسته؟
        // lockoutOnFailure: false = اگه رمز اشتباه بود، حساب قفل نشه
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            // اگه ادمین بود، بفرست پنل ادمین
            if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            // اگه عضو عادی بود، بفرست داشبورد عضو (یا همون صفحه‌ای که میخواست)
            return Redirect(model.ReturnUrl ?? "/member");
        }

        // اگه ورود ناموفق بود، پیام خطا نشون بده
        ModelState.AddModelError("", "ایمیل یا رمز عبور اشتباه است");
        return View(model);
    }

    // ════════════════════════════════════════════════════════════════
    //   ثبت‌نام
    // ════════════════════════════════════════════════════════════════

    // نمایش فرم ثبت‌نام — GET
    [HttpGet]
    public IActionResult Register()
    {
        ViewData["Title"] = "ثبت‌نام";
        return View(new RegisterViewModel());
    }

    // پردازش فرم ثبت‌نام — POST
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        ViewData["Title"] = "ثبت‌نام";

        // اگه فرم ناقص هست، دوباره نشون بده
        if (!ModelState.IsValid) return View(model);

        // یه کاربر جدید میسازیم با اطلاعاتی که وارد کرده
        var user = new ApplicationUser
        {
            UserName      = model.Email,       // username = ایمیل
            Email         = model.Email,
            FirstName     = model.FirstName,
            LastName      = model.LastName,
            PhoneNumber   = model.PhoneNumber,
            EmailConfirmed = true              // ایمیل رو تأیید‌شده فرض میکنیم (بدون ایمیل تأیید)
        };

        // کاربر رو توی دیتابیس ذخیره میکنیم با رمز عبور (هش میشه خودکار)
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // نقش "Member" رو به این کاربر میدیم
            await _userManager.AddToRoleAsync(user, "Member");

            // خودکار لاگینش میکنیم بعد از ثبت‌نام
            await _signInManager.SignInAsync(user, false);

            // میفرستیمش داشبورد عضو
            return RedirectToAction("Index", "Dashboard", new { area = "Member" });
        }

        // اگه خطا بود (مثلاً رمز ضعیفه)، خطاها رو نشون بده
        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View(model);
    }

    // ════════════════════════════════════════════════════════════════
    //   خروج
    // ════════════════════════════════════════════════════════════════

    // خروج از حساب — فقط POST (امنیت بیشتر، جلوگیری از logout اتفاقی)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // کوکی لاگین رو پاک میکنیم
        await _signInManager.SignOutAsync();

        // میفرستیم صفحه اصلی
        return RedirectToAction("Index", "Home");
    }

    // ── صفحه "دسترسی ممنوع" — وقتی کاربر سعی میکنه جایی بره که حق نداره
    public IActionResult AccessDenied() => View();
}
