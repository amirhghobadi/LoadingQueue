namespace LoadingQueue.Models;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// موجودیت راننده. هر راننده می‌تونه چند نوبت (Queue) داشته باشه.
/// حذف رانندگان به‌صورت Soft Delete انجام می‌شه (IsDeleted) تا سابقه‌ی
/// نوبت‌های قبلیش از بین نره.
/// </summary>
public class Driver
{

    [Key]
    public int Id { get; set; }

    /// <summary>این راننده متعلق به کدوم شرکت حمل‌ونقله (چندشرکتی)</summary>
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    /// <summary>نام کامل راننده</summary>
    [Required(ErrorMessage = "لطفاً {0} را وارد کنید.")]
    [Display(Name = "نام")]
    public string FullName { get; set; } = null!;

    /// <summary>شماره کارت (۱۶ رقمی) یا شماره شبا (IR + ۲۴ رقم)</summary>
    [Display(Name = "شماره کارت")]
    public string CardNumberOrSheba { get; set; } = null!;

    /// <summary>شماره پلاک ماشین، کلید جستجوی اصلی در بخش صف بارگیری</summary>
    [Required(ErrorMessage = "لطفاً {0} را وارد کنید.")]
    [Display(Name = "شماره ماشین")]
    public string CarNumber { get; set; } = null!;

    [Required(ErrorMessage = "لطفاً {0} را وارد کنید.")]
    [Display(Name = "شماره تماس")]
    public string PhoneNumber { get; set; } = null!;

    /// <summary>تاریخ ثبت راننده در سیستم</summary>
    [Display(Name = "تاریخ ایجاد")]
    [DataType(DataType.Date)]
    public DateTime? RegisterDate { get; set; } = DateTime.Now;

    // -------------------- Soft Delete --------------------
    // به‌جای حذف واقعی رکورد از دیتابیس، این پرچم true می‌شه
    // تا نوبت‌های قبلی راننده (Queue) که به Id او وصل هستن خراب نشن.

    /// <summary>آیا راننده حذف شده؟ (حذف نرم)</summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>تاریخ حذف (در صورت حذف شدن)</summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>لیست نوبت‌های ثبت‌شده برای این راننده</summary>
    public ICollection<Queue> Queues { get; set; } = new List<Queue>();
}