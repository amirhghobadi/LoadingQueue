using LoadingQueue.Models;
using LoadingQueue.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoadingQueue.Controllers;

/// <summary>
/// مدیریت شرکت‌ها - فقط مدیر کل سیستم بهش دسترسی داره، چون تعریف
/// شرکت جدید یعنی یه Tenant جدید تو سیستم ساخته می‌شه.
/// </summary>
[Authorize(Roles = "SystemAdmin")]
public class CompanyController : Controller
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyController(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<IActionResult> Index()
    {
        var companies = await _companyRepository.GetAllAsync();
        return View(companies);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Company company)
    {
        if (!ModelState.IsValid)
            return View(company);

        if (await _companyRepository.ExistsByNameAsync(company.Name, null))
        {
            ModelState.AddModelError(nameof(company.Name), "شرکتی با این نام قبلاً ثبت شده است.");
            return View(company);
        }

        await _companyRepository.AddAsync(company);
        TempData["Success"] = "شرکت با موفقیت ثبت شد.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var company = await _companyRepository.GetByIdAsync(id);
        if (company == null) return NotFound();
        return View(company);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Company company)
    {
        if (!ModelState.IsValid)
            return View(company);

        if (await _companyRepository.ExistsByNameAsync(company.Name, company.Id))
        {
            ModelState.AddModelError(nameof(company.Name), "شرکت دیگری با این نام ثبت شده است.");
            return View(company);
        }

        await _companyRepository.UpdateAsync(company);
        TempData["Success"] = "تغییرات ذخیره شد.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var company = await _companyRepository.GetByIdAsync(id);
        if (company == null) return NotFound();
        return View(company);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleActive(int id)
    {
        await _companyRepository.ToggleActiveAsync(id);
        return Ok();
    }
}