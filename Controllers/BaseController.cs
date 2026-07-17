using LoadingQueue.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LoadingQueue.Controllers;

/// <summary>
/// پایه‌ی مشترک برای کنترلرهایی که نیاز دارن بدونن کاربر فعلی به
/// کدوم شرکت وصله (برای فیلتر کردن داده‌ها بر اساس شرکت).
/// </summary>
public abstract class BaseController : Controller
{
    protected readonly UserManager<ApplicationUser> UserManager;

    protected BaseController(UserManager<ApplicationUser> userManager)
    {
        UserManager = userManager;
    }

    /// <summary>
    /// null یعنی کاربر فعلی SystemAdmin (بدون محدودیت شرکت، همه‌چیز رو می‌بینه)
    /// عدد یعنی کاربر فقط باید داده‌ی همین شرکت رو ببینه
    /// </summary>
    protected async Task<int?> GetCurrentCompanyIdAsync()
    {
        var user = await UserManager.GetUserAsync(User);
        return user?.CompanyId;
    }
}