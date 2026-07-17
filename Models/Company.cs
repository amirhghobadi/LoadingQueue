using System.ComponentModel.DataAnnotations;

namespace LoadingQueue.Models;

/// <summary>
/// شرکت حمل‌ونقل (تننت سیستم). هر شرکت رانندگان، باربری‌ها و نوبت‌های
/// جدای خودش رو داره. SystemAdmin به هیچ شرکتی وصل نیست و همه رو می‌بینه؛
/// بقیه‌ی نقش‌ها فقط داده‌ی همین Company خودشون رو می‌بینن.
/// </summary>
public class Company
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "لطفاً {0} را وارد کنید.")]
    [Display(Name = "نام شرکت")]
    public string Name { get; set; } = null!;

    [Display(Name = "شناسه ملی")]
    public string? NationalId { get; set; }

    [Display(Name = "توضیحات")]
    public string? Description { get; set; }

    [Display(Name = "تلفن")]
    public string? Phone { get; set; }

    [Display(Name = "آدرس")]
    public string? Address { get; set; }

    [Display(Name = "مسیر لوگو")]
    public string? LogoPath { get; set; }

    [Display(Name = "فعال است؟")]
    public bool IsActive { get; set; } = true;

    /// <summary>کد کوتاه شرکت، برای ساخت شماره‌ی نوبت (مثلاً "A") - باید یکتا باشه</summary>
    [Display(Name = "کد شرکت")]
    [MaxLength(3)]
    public string Code { get; set; } = "A";

    [Display(Name = "تاریخ ثبت")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
    public ICollection<Queue> Queues { get; set; } = new List<Queue>();
    public ICollection<ShippingCompany> ShippingCompanies { get; set; } = new List<ShippingCompany>();
}