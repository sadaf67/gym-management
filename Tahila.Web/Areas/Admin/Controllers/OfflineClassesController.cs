using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tahila.Application.Services.Interfaces;
using Tahila.Domain.Entities;
using Tahila.Infrastructure.FileUpload;

namespace Tahila.Web.Areas.Admin.Controllers;

// ─────────────────────────────────────────────────────────────────────────────
// این کنترلر پنل ادمین کلاس‌های آفلاین رو مدیریت میکنه
// دو بخش داره: ۱) مدیریت دوره‌ها (OfflineClass)  ۲) مدیریت جلسات (ClassSession)
// هر دوره میتونه چند جلسه داشته باشه — مثلاً دوره "کاهش وزن" → ۱۰ جلسه ویدیویی
// آدرس: /Admin/OfflineClasses
// ─────────────────────────────────────────────────────────────────────────────
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class OfflineClassesController : Controller
{
    private readonly IOfflineClassService _service;
    private readonly FileUploadService    _fileUpload;

    public OfflineClassesController(IOfflineClassService service, FileUploadService fileUpload)
    {
        _service    = service;
        _fileUpload = fileUpload;
    }

    // ════════════════════════════════════════════════════════════════
    //   مدیریت دوره‌ها (OfflineClass)
    // ════════════════════════════════════════════════════════════════

    // لیست همه دوره‌ها
    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "مدیریت کلاس‌های آفلاین";
        return View(await _service.GetAllAdminAsync());
    }

    // فرم دوره جدید
    public IActionResult Create()
    {
        ViewData["PageTitle"] = "دوره جدید";
        return View(new OfflineClass
        {
            IsActive = true,    // پیشفرض: فعال
            Level    = "مبتدی" // پیشفرض: سطح مبتدی
        });
    }

    // ذخیره دوره جدید
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OfflineClass model, IFormFile? thumbnailFile)
    {
        ViewData["PageTitle"] = "دوره جدید";
        try
        {
            // اگه عکس کاور دوره آپلود شده، ذخیره کن
            if (thumbnailFile != null)
                model.ThumbnailUrl = await _fileUpload.UploadImageAsync(thumbnailFile, "images/courses");

            await _service.CreateAsync(model);
            TempData["Success"] = "دوره با موفقیت اضافه شد.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; return View(model); }
    }

    // فرم ویرایش دوره
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["PageTitle"] = "ویرایش دوره";
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    // ذخیره تغییرات دوره
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(OfflineClass model, IFormFile? thumbnailFile)
    {
        ViewData["PageTitle"] = "ویرایش دوره";
        try
        {
            if (thumbnailFile != null)
                model.ThumbnailUrl = await _fileUpload.UploadImageAsync(thumbnailFile, "images/courses");

            await _service.UpdateAsync(model);
            TempData["Success"] = "دوره با موفقیت ویرایش شد.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; return View(model); }
    }

    // حذف دوره (حذف نرم)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        TempData["Success"] = "دوره حذف شد.";
        return RedirectToAction(nameof(Index));
    }

    // ════════════════════════════════════════════════════════════════
    //   مدیریت جلسات (ClassSession)
    // ════════════════════════════════════════════════════════════════

    // لیست جلسات یه دوره خاص
    // courseId = ID دوره‌ای که میخوایم جلساتش رو ببینیم
    public async Task<IActionResult> Sessions(int courseId)
    {
        ViewData["PageTitle"] = "جلسات دوره";
        var course = await _service.GetByIdAsync(courseId);
        if (course == null) return NotFound();

        // اطلاعات دوره رو به View میدیم تا بتونه نام دوره رو نشون بده
        ViewBag.Course = course;

        // جلسات رو بر اساس Order مرتب میکنیم
        return View(course.Sessions.OrderBy(s => s.Order).ToList());
    }

    // فرم جلسه جدید
    public async Task<IActionResult> AddSession(int courseId)
    {
        ViewData["PageTitle"] = "جلسه جدید";
        var course = await _service.GetByIdAsync(courseId);
        if (course == null) return NotFound();

        ViewBag.CourseId   = courseId;
        ViewBag.CourseName = course.Title;

        // شماره جلسه بعدی رو خودکار حساب میکنیم
        // اگه ۳ جلسه داریم، جلسه بعدی شماره ۴ میشه
        var nextSession = (course.Sessions.Any() ? course.Sessions.Max(s => s.SessionNumber) : 0) + 1;

        return View(new ClassSession
        {
            OfflineClassId  = courseId,
            SessionNumber   = nextSession, // شماره خودکار
            IsActive        = true,
            DurationMinutes = 30           // پیشفرض: ۳۰ دقیقه
        });
    }

    // ذخیره جلسه جدید
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSession(ClassSession model, IFormFile? thumbnailFile)
    {
        ViewData["PageTitle"] = "جلسه جدید";
        ViewBag.CourseId = model.OfflineClassId;
        try
        {
            // اگه تصویر جلسه آپلود شده، ذخیره کن
            if (thumbnailFile != null)
                model.ThumbnailUrl = await _fileUpload.UploadImageAsync(thumbnailFile, "images/sessions");

            // ترتیب نمایش رو برابر شماره جلسه میزاریم
            model.Order = model.SessionNumber;

            await _service.CreateSessionAsync(model);
            TempData["Success"] = "جلسه با موفقیت اضافه شد.";

            // برگرد به صفحه لیست جلسات همین دوره
            return RedirectToAction(nameof(Sessions), new { courseId = model.OfflineClassId });
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; return View(model); }
    }

    // فرم ویرایش جلسه
    public async Task<IActionResult> EditSession(int sessionId)
    {
        ViewData["PageTitle"] = "ویرایش جلسه";
        var session = await _service.GetSessionAsync(sessionId);
        if (session == null) return NotFound();

        // اطلاعات دوره والد رو هم میدیم برای نمایش breadcrumb
        ViewBag.CourseId   = session.OfflineClassId;
        ViewBag.CourseName = session.OfflineClass?.Title;
        return View(session);
    }

    // ذخیره تغییرات جلسه
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSession(ClassSession model, IFormFile? thumbnailFile)
    {
        ViewData["PageTitle"] = "ویرایش جلسه";
        ViewBag.CourseId = model.OfflineClassId;
        try
        {
            if (thumbnailFile != null)
                model.ThumbnailUrl = await _fileUpload.UploadImageAsync(thumbnailFile, "images/sessions");

            await _service.UpdateSessionAsync(model);
            TempData["Success"] = "جلسه با موفقیت ویرایش شد.";
            return RedirectToAction(nameof(Sessions), new { courseId = model.OfflineClassId });
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; return View(model); }
    }

    // حذف جلسه — courseId رو هم میگیره تا بعد از حذف بره لیست جلسات همون دوره
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSession(int sessionId, int courseId)
    {
        await _service.DeleteSessionAsync(sessionId);
        TempData["Success"] = "جلسه حذف شد.";
        return RedirectToAction(nameof(Sessions), new { courseId });
    }
}
