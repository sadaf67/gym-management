using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;

namespace Tahila.Web.Areas.Admin.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر "لیست پرداخت‌ها" رو در پنل ادمین نمایش میده
// آدرس: /Admin/Payments
// ادمین میتونه همه تراکنش‌ها رو ببینه — موفق، ناموفق، در انتظار
// ─────────────────────────────────────────────────────────────────────────────
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class PaymentsController : Controller
{
    // سرویس پرداخت — برای گرفتن لیست همه تراکنش‌ها
    private readonly IPaymentService _service;

    public PaymentsController(IPaymentService service) => _service = service;

    // ── لیست همه پرداخت‌ها ──────────────────────────────────────────────────
    // ادمین فقط میتونه ببینه — ویرایش یا حذف نداریم (تراکنش مالی حذف نمیشه)
    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "پرداخت‌ها";

        // همه پرداخت‌ها رو بیار — صف زمانی از جدیدترین به قدیمی‌ترین
        return View(await _service.GetAllPaymentsAsync());
    }
}
