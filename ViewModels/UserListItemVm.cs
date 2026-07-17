namespace LoadingQueue.ViewModels;

public class UserListItemVm
{
    public string Id { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsActive { get; set; }
    public string Role { get; set; } = null!;
    public string? CompanyName { get; set; }
}