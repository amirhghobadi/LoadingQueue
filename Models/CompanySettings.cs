using System.ComponentModel.DataAnnotations;

namespace LoadingQueue.Models;

/// <summary>
/// تنظیمات هر شرکت. فقط شامل چیزهاییه که واقعاً رو رفتار سیستم
/// اثر می‌ذارن؛ تنظیمات نمایشی/تزئینی (که هنوز قابلیت واقعی پشتشون
/// نیست، مثل پیامک/ایمیل) عمداً اینجا نیستن.
/// </summary>
public class CompanySettings
{
    [Key]
    public int Id { get; set; }

    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    /// <summary>اگه فعال باشه، فقط مدیر شرکت (نه کاربر عادی) می‌تونه نوبت رو «تکمیل‌شده» کنه</summary>
    [Display(Name = "نیاز به تایید مدیر برای تکمیل نوبت")]
    public bool RequireApprovalForCompletion { get; set; } = false;

    /// <summary>تعداد ردیف پیش‌فرض هر صفحه‌ی جدول صف بارگیری</summary>
    [Display(Name = "تعداد ردیف هر صفحه")]
    [Range(10, 500)]
    public int DefaultPageSize { get; set; } = 100;
}