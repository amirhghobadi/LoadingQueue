using LoadingQueue.Models;
using LoadingQueue.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LoadingQueue.Controllers;

public class NotificationController : BaseController
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationController(INotificationRepository notificationRepository, UserManager<ApplicationUser> userManager)
        : base(userManager)
    {
        _notificationRepository = notificationRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetLatest()
    {
        var companyId = await GetCurrentCompanyIdAsync();

        if (companyId == null)
            return Json(new { unreadCount = 0, items = new List<object>() });

        var notifications = await _notificationRepository.GetRecentAsync(companyId.Value, 5);
        var unreadCount = await _notificationRepository.GetUnreadCountAsync(companyId.Value);

        return Json(new
        {
            unreadCount,
            items = notifications.Select(n => new
            {
                title = n.Title,
                message = n.Message,
                time = n.CreatedAt.ToString("HH:mm"),
                isRead = n.IsRead
            })
        });
    }

    [HttpPost]
    public async Task<IActionResult> MarkAllRead()
    {
        var companyId = await GetCurrentCompanyIdAsync();

        if (companyId.HasValue)
            await _notificationRepository.MarkAllAsReadAsync(companyId.Value);

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> ClearAll()
    {
        var companyId = await GetCurrentCompanyIdAsync();

        if (companyId.HasValue)
            await _notificationRepository.ClearAllAsync(companyId.Value);

        return Ok();
    }
}