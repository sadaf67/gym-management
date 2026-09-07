using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tahila.Application.Services.Interfaces;
using Tahila.Infrastructure.Data;

namespace Tahila.Web.Areas.Member.Controllers;

[Area("Member")]
[Authorize]
public class DashboardController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPaymentService _paymentService;
    private readonly TahilaDbContext _db;

    public DashboardController(UserManager<ApplicationUser> userManager, IPaymentService paymentService, TahilaDbContext db)
    { _userManager = userManager; _paymentService = paymentService; _db = db; }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "پنل کاربری";
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account", new { area = "" });

        ViewBag.User = user;
        ViewBag.Payments = await _paymentService.GetUserPaymentsAsync(user.Id);

        // BMI history for chart (last 10 records)
        var bmiHistory = await _db.BodyAnalyses
            .Where(b => b.UserId == user.Id && !b.IsDeleted)
            .OrderByDescending(b => b.CreatedAt)
            .Take(10)
            .ToListAsync();
        bmiHistory.Reverse();
        ViewBag.BmiHistory = bmiHistory;

        // Streak: count consecutive days with BMI records
        var allDates = await _db.BodyAnalyses
            .Where(b => b.UserId == user.Id && !b.IsDeleted)
            .Select(b => b.CreatedAt.Date)
            .Distinct()
            .OrderByDescending(d => d)
            .ToListAsync();

        int streak = 0;
        var today = DateTime.Today;
        for (int i = 0; i < allDates.Count; i++)
        {
            if (allDates[i] == today.AddDays(-i)) streak++;
            else break;
        }
        ViewBag.Streak = streak;

        return View();
    }
}
