using System.ComponentModel.DataAnnotations;

namespace LoadingQueue.Models;

public enum ShippingCompanyStatus
{
    [Display(Name = "فعال")]
    Active = 1,

    [Display(Name = "غیرفعال")]
    Inactive = 2,

    [Display(Name = "در انتظار تایید")]
    PendingApproval = 3
}

/// <summary>
/// باربری (طرف‌حساب/مشتری شرکت حمل‌ونقل). قبلاً این فقط یه رشته‌ی
/// آزاد با ۳ گزینه‌ی ثابت روی Queue بود؛ الان یه Entity کامل شد تا
/// بشه براش کد ملی، مدیرعامل، تماس، لوگو و وضعیت نگه داشت.
/// </summary>
public class ShippingCompany
{
    [Key]
    public int Id { get; set; }

    /// <summary>این باربری متعلق به کدوم شرکت حمل‌ونقله</summary>
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    [Required(ErrorMessage = "لطفاً {0} را وارد کنید.")]
    [Display(Name = "نام باربری")]
    public string Name { get; set; } = null!;

    [Display(Name = "کد ملی / شناسه")]
    public string? NationalId { get; set; }

    [Display(Name = "نام مدیرعامل")]
    public string? ManagerName { get; set; }

    [Required(ErrorMessage = "لطفاً {0} را وارد کنید.")]
    [Display(Name = "شماره موبایل")]
    public string MobilePhone { get; set; } = null!;

    [Display(Name = "تلفن ثابت")]
    public string? LandlinePhone { get; set; }

    [Display(Name = "آدرس")]
    public string? Address { get; set; }

    [Display(Name = "مسیر لوگو")]
    public string? LogoPath { get; set; }

    [Display(Name = "وضعیت")]
    public ShippingCompanyStatus Status { get; set; } = ShippingCompanyStatus.Active;

    [Display(Name = "تاریخ ثبت")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Queue> Queues { get; set; } = new List<Queue>();
}