using Microsoft.AspNetCore.Identity;

namespace LoadingQueue.Models;

/// <summary>
/// کاربر سیستم (روی Identity ساخته شده). هر کاربر (به‌جز مدیر کل سیستم)
/// به یه Company وصله؛ Company = null یعنی مدیر کل سیستم (SystemAdmin)
/// که به هیچ شرکتی محدود نیست و همه‌ی شرکت‌ها رو می‌بینه.
/// </summary>
public class ApplicationUser : IdentityUser
{
    [PersonalData]
    public string FullName { get; set; } = null!;

    /// <summary>null یعنی SystemAdmin (به هیچ شرکتی محدود نیست)</summary>
    public int? CompanyId { get; set; }
    public Company? Company { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}