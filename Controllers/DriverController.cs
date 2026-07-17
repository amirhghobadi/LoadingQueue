using System.Diagnostics;
using LoadingQueue.Models;
using LoadingQueue.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LoadingQueue.Controllers;

public class DriverController : BaseController
{
    private readonly IDriverRepository _driverRepository;
    private readonly INotificationRepository _notificationRepository;

    public DriverController(
        IDriverRepository driverRepository,
        INotificationRepository notificationRepository,
        Microsoft.AspNetCore.Identity.UserManager<Models.ApplicationUser> userManager)
        : base(userManager)
    {
        _driverRepository = driverRepository;
        _notificationRepository = notificationRepository;
    }

    #region Index

    /// <summary>لیست همه‌ی رانندگان (Views/Driver/Index.cshtml)</summary>
    public async Task<IActionResult> Index()
    {
        var companyId = await GetCurrentCompanyIdAsync();
        var drivers = await _driverRepository.GetAllAsync(companyId);
        return View(drivers);
    }

    #endregion

    #region Search (AJAX - استفاده‌شده در بخش صف بارگیری، Queue/Index.cshtml)

    [HttpGet]
    public async Task<IActionResult> Search(string carNumber)
    {
        if (string.IsNullOrWhiteSpace(carNumber))
            return Json(new List<object>());

        carNumber = carNumber.Trim();

        var companyId = await GetCurrentCompanyIdAsync();
        var drivers = await _driverRepository.SearchByCarNumber(carNumber, companyId);

        return Json(drivers.Select(d => new
        {
            id = d.Id,
            fullName = d.FullName,
            carNumber = d.CarNumber,
            phoneNumber = d.PhoneNumber,
            cardNumber = d.CardNumberOrSheba
        }));
    }

    #endregion

    #region Create

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Driver driver)
    {

        ModelState.Remove("Company");
        ModelState.Remove("CompanyId");
        
        if (!ModelState.IsValid)
        {          
            return View(driver);
        }


        var companyId = await GetCurrentCompanyIdAsync();

        if (companyId == null)
        {
            ModelState.AddModelError(string.Empty, "مدیر کل سیستم نمی‌تواند مستقیم راننده ثبت کند؛ باید از طریق مدیر یک شرکت مشخص انجام شود.");
            return View(driver);
        }

        driver.CompanyId = companyId.Value;

        if (await _driverRepository.ExistsAsync(driver))
        {
            ModelState.AddModelError(nameof(driver.CarNumber), "راننده‌ای با این شماره ماشین قبلاً ثبت شده است.");
            return View(driver);
        }

        await _driverRepository.AddAsync(driver);

        await _notificationRepository.AddAsync(
            companyId.Value,
            "راننده جدید ثبت شد",
            $"راننده «{driver.FullName}» با شماره ماشین {driver.CarNumber} ثبت شد.");

        TempData["Success"] = "راننده با موفقیت ثبت شد.";

        return RedirectToAction(nameof(Index));
    }

    #endregion

    #region Edit

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var companyId = await GetCurrentCompanyIdAsync();
        var driver = await _driverRepository.GetByIdAsync(id, companyId);
        if (driver == null) return NotFound();
        return View(driver);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Driver driver)
    {
        if (!ModelState.IsValid)
            return View(driver);

        if (await _driverRepository.ExistsAsync(driver))
        {
            ModelState.AddModelError(
                nameof(driver.CarNumber),
                "راننده‌ی دیگری با این شماره ماشین ثبت شده است.");

            return View(driver);
        }

        await _driverRepository.UpdateAsync(driver);

        TempData["Success"] = "تغییرات با موفقیت ذخیره شد.";

        return RedirectToAction(nameof(Index));
    }

    #endregion

    #region Details

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var companyId = await GetCurrentCompanyIdAsync();
        var driver = await _driverRepository.GetByIdAsync(id, companyId);
        if (driver == null) return NotFound();
        return View(driver);
    }

    #endregion

    #region Delete (Soft Delete - از دکمه‌ی حذف در Views/Driver/Index.cshtml با AJAX صدا زده می‌شه)

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _driverRepository.DeleteAsync(id);

        return Ok();
    }

    #endregion
}