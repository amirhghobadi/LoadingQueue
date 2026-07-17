using LoadingQueue.Models;
using LoadingQueue.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LoadingQueue.Controllers;

/// <summary>
/// مدیریت باربری‌ها. برخلاف Company (که مخصوص SystemAdmin بود)، این
/// صفحه برای مدیر هر شرکت (CompanyAdmin) هم بازه، چون باربری‌ها
/// طرف‌حساب‌های خود همون شرکتن، نه یه چیز سراسری.
/// </summary>
[Authorize(Roles = "SystemAdmin,CompanyAdmin")]
public class ShippingCompanyController : BaseController
{
    private readonly IShippingCompanyRepository _shippingCompanyRepository;

    public ShippingCompanyController(
        IShippingCompanyRepository shippingCompanyRepository,
        UserManager<ApplicationUser> userManager)
        : base(userManager)
    {
        _shippingCompanyRepository = shippingCompanyRepository;
    }

    public async Task<IActionResult> Index()
    {
        var companyId = await GetCurrentCompanyIdAsync();
        var list = await _shippingCompanyRepository.GetAllAsync(companyId);
        return View(list);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ShippingCompany shippingCompany)
    {
        var companyId = await GetCurrentCompanyIdAsync();

        if (companyId == null)
        {
            ModelState.AddModelError(string.Empty, "مدیر کل سیستم نمی‌تواند مستقیم باربری ثبت کند.");
            return View(shippingCompany);
        }

        // این‌ها توسط سرور تعیین می‌شن، نه از فرم (برای امنیت)
        ModelState.Remove(nameof(ShippingCompany.CompanyId));
        ModelState.Remove(nameof(ShippingCompany.Company));

        if (!ModelState.IsValid)
            return View(shippingCompany);

        shippingCompany.CompanyId = companyId.Value;

        if (await _shippingCompanyRepository.ExistsByNameAsync(shippingCompany.Name, companyId.Value, null))
        {
            ModelState.AddModelError(nameof(shippingCompany.Name), "باربری‌ای با این نام قبلاً ثبت شده است.");
            return View(shippingCompany);
        }

        await _shippingCompanyRepository.AddAsync(shippingCompany);
        TempData["Success"] = "باربری با موفقیت ثبت شد.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var companyId = await GetCurrentCompanyIdAsync();
        var shippingCompany = await _shippingCompanyRepository.GetByIdAsync(id, companyId);
        if (shippingCompany == null) return NotFound();
        return View(shippingCompany);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ShippingCompany shippingCompany)
    {
        ModelState.Remove(nameof(ShippingCompany.CompanyId));
        ModelState.Remove(nameof(ShippingCompany.Company));

        if (!ModelState.IsValid)
            return View(shippingCompany);

        var companyId = await GetCurrentCompanyIdAsync();

        // مطمئن می‌شیم این باربری واقعاً مال همین شرکته (جلوگیری از دستکاری Id تو URL)
        var existing = await _shippingCompanyRepository.GetByIdAsync(shippingCompany.Id, companyId);
        if (existing == null) return NotFound();

        shippingCompany.CompanyId = existing.CompanyId;

        if (await _shippingCompanyRepository.ExistsByNameAsync(shippingCompany.Name, existing.CompanyId, shippingCompany.Id))
        {
            ModelState.AddModelError(nameof(shippingCompany.Name), "باربری دیگری با این نام ثبت شده است.");
            return View(shippingCompany);
        }

        await _shippingCompanyRepository.UpdateAsync(shippingCompany);
        TempData["Success"] = "تغییرات ذخیره شد.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var companyId = await GetCurrentCompanyIdAsync();
        var shippingCompany = await _shippingCompanyRepository.GetByIdAsync(id, companyId);
        if (shippingCompany == null) return NotFound();
        return View(shippingCompany);
    }

    /// <summary>لیست اسم باربری‌های فعال - برای dropdown تو صفحه‌ی صف بارگیری</summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetActiveNames()
    {
        var companyId = await GetCurrentCompanyIdAsync();
        var names = await _shippingCompanyRepository.GetActiveNamesAsync(companyId);
        return Json(names);
    }
}