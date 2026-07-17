using System;
using System.ComponentModel.DataAnnotations;

namespace LoadingQueue.Models;




/// <summary>
/// موجودیت نوبت بارگیری. هر رکورد نشون‌دهنده‌ی یک نوبتیه که یک راننده
/// برای بارگیری در یک تاریخ/ساعت مشخص گرفته.
/// </summary>
public enum QueueStatus
{
    [Display(Name = "در انتظار")]
    Pending = 1,

    [Display(Name = "در حال بارگیری")]
    InProgress = 2,

    [Display(Name = "تکمیل شده")]
    Completed = 3,

    [Display(Name = "لغو شده")]
    Cancelled = 4
}

public class Queue
{
    [Key]
    public int Id { get; set; }

    /// <summary>شماره‌ی نوبت فرمت‌دار، مثل A-1403-0001 (موقع ثبت خودکار ساخته می‌شه)</summary>
    [Display(Name = "شماره نوبت")]
    public string QueueNumber { get; set; } = "";

    [Display(Name = "وضعیت")]
    public QueueStatus Status { get; set; } = QueueStatus.Pending;

    /// <summary>این نوبت متعلق به کدوم شرکت حمل‌ونقله (چندشرکتی)</summary>
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    /// <summary>
    /// ارتباط واقعی با Entity باربری. فعلاً nullable و در UI استفاده
    /// نمی‌شه (چون dropdown فعلی هنوز رشته‌ای کار می‌کنه) — تو فاز ۳
    /// که صفحه‌ی مدیریت باربری‌ها رو می‌سازیم، این رسماً وصل می‌شه.
    /// </summary>
    public int? ShippingCompanyId { get; set; }
    public ShippingCompany? ShippingCompany { get; set; }

    [Display(Name = "نام باربری")]
    public string? ShippingCompanyName { get; set; }

    [Display(Name = "شماره بارنامه")]
    public string? WaybillNumber { get; set; }

    [Display(Name = "مبلغ کرایه")]
    public decimal? FreightAmount { get; set; }

    [Display(Name = "مقصد")]
    public string? Destination { get; set; }

    /// <summary>ساعت نوبت به فرمت HH:mm</summary>
    [Display(Name = "ساعت")]
    public TimeSpan? QueueTime { get; set; }

    [Display(Name = "کارتن کیک")]
    public int? CakeCartonCount { get; set; }

    [Display(Name = "کارتن آجیل")]
    public int? NutCartonCount { get; set; }

    [Display(Name = "شماره خروجی")]
    public string? ExitNumber { get; set; }

    [Display(Name = "تاریخ ایجاد")]
    public DateTime QueueDate { get; set; }

    /// <summary>
    /// ترتیب نمایش دستی (با Drag and Drop قابل تغییره). این با QueueTime
    /// فرق داره: QueueTime ساعت واقعیه، این فقط ترتیب نمایش تو جدوله.
    /// </summary>
    [Display(Name = "ترتیب نمایش")]
    public int SortOrder { get; set; }

    // -------------------- ارتباط با راننده --------------------

    /// <summary>کلید خارجی به Driver</summary>
    public int DriverId { get; set; }

    public Driver? Driver { get; set; }

    // این ۴ فیلد زیر عمداً از اطلاعات Driver کپی (Snapshot) می‌شن.
    // دلیل: اگه بعداً مشخصات راننده در جدول Driver عوض بشه (مثلاً
    // شماره کارتش)، نوبت‌های قبلی همون اطلاعاتی که موقع ثبت بوده رو
    // نگه می‌دارن و عوض نمی‌شن. این تکراری بودن، باگ نیست، عمدیه.

    [Display(Name = "نام راننده")]
    public string? DriverName { get; set; }

    [Display(Name = "شماره تماس راننده")]
    public string? DriverPhone { get; set; }

    [Display(Name = "شماره ماشین راننده")]
    public string? DriverCarNumber { get; set; }

    [Display(Name = "شماره کارت راننده")]
    public string? DriverCardNumber { get; set; }
}