using System.ComponentModel.DataAnnotations;

namespace LoadingQueue.Models;

/// <summary>اعلان‌های داخل سیستم (زنگوله‌ی بالای صفحه). فقط برای همون شرکتی که رخداد توش افتاده.</summary>
public class Notification
{
    [Key]
    public int Id { get; set; }

    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    [Display(Name = "عنوان")]
    public string Title { get; set; } = null!;

    [Display(Name = "پیام")]
    public string Message { get; set; } = null!;

    [Display(Name = "تاریخ ایجاد")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Display(Name = "خوانده شده؟")]
    public bool IsRead { get; set; } = false;
}