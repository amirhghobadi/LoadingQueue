using System.ComponentModel.DataAnnotations;
using LoadingQueue.Models;

namespace LoadingQueue.ViewModels;

public class UserCreateVm
{
    [Required(ErrorMessage = "لطفاً {0} را وارد کنید.")]
    [Display(Name = "نام و نام خانوادگی")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "لطفاً {0} را وارد کنید.")]
    [EmailAddress(ErrorMessage = "ایمیل معتبر نیست.")]
    [Display(Name = "ایمیل (نام کاربری)")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "لطفاً {0} را وارد کنید.")]
    [DataType(DataType.Password)]
    [Display(Name = "رمز عبور")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "لطفاً {0} را انتخاب کنید.")]
    [Display(Name = "نقش")]
    public string Role { get; set; } = null!;

    [Display(Name = "شرکت")]
    public int? CompanyId { get; set; }

    [Display(Name = "فعال است؟")]
    public bool IsActive { get; set; } = true;

    public List<Company>? Companies { get; set; }
}