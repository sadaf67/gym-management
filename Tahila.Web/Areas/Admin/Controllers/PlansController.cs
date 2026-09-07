using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;

namespace Tahila.Web.Areas.Admin.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر "پکیج‌های عضویت" رو در پنل ادمین مدیریت میکنه
// آدرس: /Admin/Plans
// ادمین میتونه پکیج‌ها رو با قیمت، مدت و امکانات تعریف کنه
// ─────────────────────────────────────────────────────────────────────────────
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class PlansController : Controller
{
    // سرویس پکیج‌ها — برای عملیات CRUD
    private readonly IPlanService _service;

    public PlansController(IPlanService service) => _service = service;

    // ── لیست همه پکیج‌ها ────────────────────────────────────────────────────
    public async Task<IActionResult> Index()
    { ViewData["PageTitle"] = "پلن‌های عضویت"; return View(await _service.GetAllAsync()); }

    // ── فرم پکیج جدید ───────────────────────────────────────────────────────
    public IActionResult Create()
    { ViewData["PageTitle"] = "پلن جدید"; return View(new Plan()); }

    // ── ذخیره پکیج جدید ─────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Plan model)
    {
        ViewData["PageTitle"] = "پلن جدید";
        if (!ModelState.IsValid) return View(model);
        await _service.CreateAsync(model);
        TempData["Success"] = "پلن با موفقیت اضافه شد.";
        return RedirectToAction(nameof(Index));
    }

    // ── فرم ویرایش پکیج ─────────────────────────────────────────────────────
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["PageTitle"] = "ویرایش پلن";
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    // ── ذخیره تغییرات پکیج ──────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Plan model)
    {
        ViewData["PageTitle"] = "ویرایش پلن";
        if (!ModelState.IsValid) return View(model);
        await _service.UpdateAsync(model);
        TempData["Success"] = "پلن با موفقیت ویرایش شد.";
        return RedirectToAction(nameof(Index));
    }

    // ── حذف پکیج ────────────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    { await _service.DeleteAsync(id); TempData["Success"] = "پلن حذف شد."; return RedirectToAction(nameof(Index)); }
}
