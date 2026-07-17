using LoadingQueue.Models;
using LoadingQueue.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LoadingQueue.Controllers;

[Authorize(Roles = "SystemAdmin,CompanyAdmin")]
public class SettingsController : BaseController
{
    private readonly ISettingsRepository _settingsRepository;

    public SettingsController(ISettingsRepository settingsRepository, UserManager<ApplicationUser> userManager)
        : base(userManager)
    {
        _settingsRepository = settingsRepository;
    }

    public async Task<IActionResult> Index()
    {
        var companyId = await GetCurrentCompanyIdAsync();

        if (companyId == null)
        {
            TempData["Error"] = "مدیر کل سیستم باید از صفحه‌ی مدیریت شرکت‌ها، شرکت مدنظر را انتخاب کند.";
            return RedirectToAction("Index", "Company");
        }

        var settings = await _settingsRepository.GetOrCreateAsync(companyId.Value);
        return View(settings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CompanySettings settings)
    {
        var companyId = await GetCurrentCompanyIdAsync();

        if (companyId == null)
            return Forbid();

        settings.CompanyId = companyId.Value;

        // مطمئن می‌شیم Id واقعاً مال همین شرکته، نه یه شرکت دیگه
        var existing = await _settingsRepository.GetOrCreateAsync(companyId.Value);
        settings.Id = existing.Id;

        await _settingsRepository.UpdateAsync(settings);
        TempData["Success"] = "تنظیمات ذخیره شد.";
        return RedirectToAction(nameof(Index));
    }
}