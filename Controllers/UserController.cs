using LoadingQueue.Models;
using LoadingQueue.Repositories;
using LoadingQueue.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoadingQueue.Controllers;

/// <summary>
/// مدیریت کاربران و سطح دسترسی - فقط مدیر کل سیستم، چون تعیین نقش
/// و اتصال کاربر به شرکت، مسئله‌ی امنیتی حساسیه.
/// </summary>
[Authorize(Roles = "SystemAdmin")]
public class UserController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICompanyRepository _companyRepository;

    private readonly INotificationRepository _notificationRepository;

    public UserController(
        UserManager<ApplicationUser> userManager,
        ICompanyRepository companyRepository,
        INotificationRepository notificationRepository)
    {
        _userManager = userManager;
        _companyRepository = companyRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.OrderBy(x => x.FullName).ToListAsync();
        var companies = await _companyRepository.GetAllAsync();

        var list = new List<UserListItemVm>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            list.Add(new UserListItemVm
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                IsActive = user.IsActive,
                Role = roles.FirstOrDefault() ?? "-",
                CompanyName = companies.FirstOrDefault(c => c.Id == user.CompanyId)?.Name
            });
        }

        return View(list);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new UserCreateVm
        {
            Companies = await _companyRepository.GetAllAsync()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateVm vm)
    {
        // نقش SystemAdmin نباید به هیچ شرکتی وصل باشه؛ بقیه‌ی نقش‌ها باید حتماً شرکت داشته باشن
        if (vm.Role != "SystemAdmin" && vm.CompanyId == null)
        {
            ModelState.AddModelError(nameof(vm.CompanyId), "برای این نقش انتخاب شرکت الزامی است.");
        }

        if (!ModelState.IsValid)
        {
            vm.Companies = await _companyRepository.GetAllAsync();
            return View(vm);
        }

        var user = new ApplicationUser
        {
            UserName = vm.Email,
            Email = vm.Email,
            FullName = vm.FullName,
            CompanyId = vm.Role == "SystemAdmin" ? null : vm.CompanyId,
            IsActive = vm.IsActive,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, vm.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            vm.Companies = await _companyRepository.GetAllAsync();
            return View(vm);
        }

        await _userManager.AddToRoleAsync(user, vm.Role);

        if (user.CompanyId.HasValue)
        {
            await _notificationRepository.AddAsync(
                user.CompanyId.Value,
                "کاربر جدید ثبت شد",
                $"کاربر «{user.FullName}» با نقش {RoleFa(vm.Role)} به سیستم اضافه شد.");
        }

        TempData["Success"] = "کاربر با موفقیت ثبت شد.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleActive(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);

        return Ok();
    }

    private static string RoleFa(string role) => role switch
    {
        "SystemAdmin" => "مدیر سیستم",
        "CompanyAdmin" => "مدیر شرکت",
        "User" => "کاربر عادی",
        "Driver" => "راننده",
        _ => role
    };
}