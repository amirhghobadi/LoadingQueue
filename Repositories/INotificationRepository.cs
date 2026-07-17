using LoadingQueue.Data;
using LoadingQueue.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadingQueue.Repositories;

public interface INotificationRepository
{
    Task<List<Notification>> GetRecentAsync(int companyId, int count);
    Task<int> GetUnreadCountAsync(int companyId);
    Task AddAsync(int companyId, string title, string message);
    Task MarkAllAsReadAsync(int companyId);
    Task ClearAllAsync(int companyId);
}

public class NotificationRepository : INotificationRepository
{
    private readonly QueueDBContext _context;

    public NotificationRepository(QueueDBContext context)
    {
        _context = context;
    }

    public async Task<List<Notification>> GetRecentAsync(int companyId, int count)
    {
        return await _context.Notifications
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(int companyId)
    {
        return await _context.Notifications
            .Where(x => x.CompanyId == companyId && !x.IsRead)
            .CountAsync();
    }

    public async Task AddAsync(int companyId, string title, string message)
    {
        _context.Notifications.Add(new Notification
        {
            CompanyId = companyId,
            Title = title,
            Message = message,
            CreatedAt = DateTime.Now,
            IsRead = false
        });

        await _context.SaveChangesAsync();
    }

    /// <summary>وقتی کاربر دراپ‌داون اعلان‌ها رو باز می‌کنه، همه به‌عنوان خونده‌شده علامت می‌خورن</summary>
    public async Task MarkAllAsReadAsync(int companyId)
    {
        await _context.Notifications
            .Where(x => x.CompanyId == companyId && !x.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsRead, true));
    }

    /// <summary>حذف کامل همه‌ی اعلان‌های این شرکت (دکمه‌ی «پاک کردن همه»)</summary>
    public async Task ClearAllAsync(int companyId)
    {
        await _context.Notifications
            .Where(x => x.CompanyId == companyId)
            .ExecuteDeleteAsync();
    }
}