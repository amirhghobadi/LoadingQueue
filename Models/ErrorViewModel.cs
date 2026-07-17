namespace LoadingQueue.Models;

/// <summary>
/// مدل پیش‌فرض صفحه‌ی خطا (Views/Shared/Error.cshtml).
/// این کلاس جزو تمپلیت پیش‌فرض ASP.NET MVC هست و قبلاً از پروژه گم شده بود.
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}