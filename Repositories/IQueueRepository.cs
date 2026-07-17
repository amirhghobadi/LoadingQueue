using LoadingQueue.Data;
using LoadingQueue.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadingQueue.Repositories;

public interface IQueueRepository
{
    Task<List<Queue>> GetAllAsync(int? companyId);
    Task<Queue?> GetByIdAsync(int id, int? companyId);
    Task AddAsync(Queue queue);
    Task UpdateAsync(Queue queue);
    Task DeleteAsync(int id);
    Task<int> GetTodayCountAsync(int? companyId);
    Task<List<Queue>> GetByDate(DateTime date, int? companyId);
    Task<List<Queue>> GetByMonth(int year, int month, int? companyId);
    Task<List<Queue>> GetBetween(DateTime from, DateTime to, int? companyId);

    Task ReorderAsync(List<int> orderedIds);
    Task<bool> ChangeStatusAsync(int id, QueueStatus status, int? companyId);

    Task<Dictionary<DateTime, (int total, int completed)>> GetLastNDaysStatsAsync(int days, int? companyId);

    Task<List<Queue>> SearchAsync(DateTime? from, DateTime? to, QueueStatus? status, string? search, int? companyId);
}

public class QueueRepository : IQueueRepository
{
    private readonly QueueDBContext _context;

    public QueueRepository(QueueDBContext context)
    {
        _context = context;
    }

    public async Task<List<Queue>> GetAllAsync(int? companyId)
    {
        return await _context.Queues
            .AsNoTracking()
            .Where(x => companyId == null || x.CompanyId == companyId)
            .OrderBy(x => x.QueueDate)
            .ThenBy(x => x.QueueTime)
            .ThenBy(x => x.Id)
            .ToListAsync();
    }

    public async Task<Queue?> GetByIdAsync(int id, int? companyId)
    {
        return await _context.Queues
            .Where(x => companyId == null || x.CompanyId == companyId)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Queue queue)
    {
        if (queue.QueueDate == default) queue.QueueDate = DateTime.Today;
        if (queue.QueueTime == default) queue.QueueTime = DateTime.Now.TimeOfDay;

        var maxOrder = await _context.Queues
            .Where(x => x.QueueDate.Date == queue.QueueDate.Date && x.CompanyId == queue.CompanyId)
            .Select(x => (int?)x.SortOrder)
            .MaxAsync();

        queue.SortOrder = (maxOrder ?? 0) + 1;

        queue.QueueNumber = await GenerateQueueNumberAsync(queue.CompanyId, queue.QueueDate);

        _context.Queues.Add(queue);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// شماره‌ی نوبت به فرمت {کد شرکت}-{سال شمسی}-{ترتیبی ۴رقمی} می‌سازه.
    /// شماره‌ی ترتیبی هر سال (شمسی) از نو شروع می‌شه، جدا برای هر شرکت.
    /// </summary>
    private async Task<string> GenerateQueueNumberAsync(int companyId, DateTime queueDate)
    {
        var company = await _context.Companies.FindAsync(companyId);
        var code = string.IsNullOrWhiteSpace(company?.Code) ? "A" : company.Code;

        var pc = new System.Globalization.PersianCalendar();
        var persianYear = pc.GetYear(queueDate);

        // بازه‌ی میلادی معادل همون سال شمسی (برای فیلتر کردن نوبت‌های همون سال)
        var yearStart = pc.ToDateTime(persianYear, 1, 1, 0, 0, 0, 0);
        var yearEnd = pc.ToDateTime(persianYear, pc.GetMonthsInYear(persianYear), pc.GetDaysInMonth(persianYear, pc.GetMonthsInYear(persianYear)), 23, 59, 59, 0);

        var countThisYear = await _context.Queues
            .Where(x => x.CompanyId == companyId)
            .Where(x => x.QueueDate >= yearStart && x.QueueDate <= yearEnd)
            .CountAsync();

        var sequence = countThisYear + 1;

        return $"{code}-{persianYear}-{sequence:0000}";
    }

    public async Task UpdateAsync(Queue queue)
    {
        _context.Entry(queue).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var queue = await _context.Queues.FindAsync(id);
        if (queue == null) return;
        _context.Queues.Remove(queue);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetTodayCountAsync(int? companyId)
    {
        var today = DateTime.Today;

        return await _context.Queues
            .Where(x => companyId == null || x.CompanyId == companyId)
            .CountAsync(x => x.QueueDate.Date == today);
    }

    public async Task<List<Queue>> GetByDate(DateTime date, int? companyId)
    {
        return await _context.Queues
            .AsNoTracking()
            .Where(x => x.QueueDate.Date == date.Date)
            .Where(x => companyId == null || x.CompanyId == companyId)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .ToListAsync();
    }

    public async Task<List<Queue>> GetByMonth(int year, int month, int? companyId)
    {
        return await _context.Queues
            .AsNoTracking()
            .Where(x => x.QueueDate.Year == year && x.QueueDate.Month == month)
            .Where(x => companyId == null || x.CompanyId == companyId)
            .OrderBy(x => x.QueueDate)
            .ThenBy(x => x.QueueTime)
            .ToListAsync();
    }

    public async Task<List<Queue>> GetBetween(DateTime from, DateTime to, int? companyId)
    {
        return await _context.Queues
            .AsNoTracking()
            .Where(x => x.QueueDate.Date >= from.Date && x.QueueDate.Date <= to.Date)
            .Where(x => companyId == null || x.CompanyId == companyId)
            .OrderBy(x => x.QueueDate)
            .ThenBy(x => x.QueueTime)
            .ToListAsync();
    }

    /// <summary>ذخیره‌ی ترتیب جدید نوبت‌ها بعد از Drag and Drop</summary>
    public async Task ReorderAsync(List<int> orderedIds)
    {
        for (int i = 0; i < orderedIds.Count; i++)
        {
            var queue = await _context.Queues.FindAsync(orderedIds[i]);

            if (queue != null)
                queue.SortOrder = i;
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>تغییر وضعیت نوبت (در انتظار/در حال بارگیری/تکمیل‌شده/لغو‌شده)</summary>
    public async Task<bool> ChangeStatusAsync(int id, QueueStatus status, int? companyId)
    {
        var queue = await _context.Queues
            .Where(x => companyId == null || x.CompanyId == companyId)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (queue == null)
            return false;

        queue.Status = status;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>آمار N روز اخیر برای نمودار داشبورد: تعداد کل نوبت و تعداد تکمیل‌شده در هر روز</summary>
    public async Task<Dictionary<DateTime, (int total, int completed)>> GetLastNDaysStatsAsync(int days, int? companyId)
    {
        var from = DateTime.Today.AddDays(-(days - 1));

        var queues = await _context.Queues
            .AsNoTracking()
            .Where(x => companyId == null || x.CompanyId == companyId)
            .Where(x => x.QueueDate.Date >= from)
            .Select(x => new { x.QueueDate, x.Status })
            .ToListAsync();

        var result = new Dictionary<DateTime, (int total, int completed)>();

        for (int i = 0; i < days; i++)
        {
            var day = from.AddDays(i);

            var dayItems = queues.Where(x => x.QueueDate.Date == day.Date).ToList();

            result[day] = (
                dayItems.Count,
                dayItems.Count(x => x.Status == QueueStatus.Completed)
            );
        }

        return result;
    }


    /// <summary>جستجوی نوبت‌ها برای صفحه‌ی گزارش‌ها، با فیلتر بازه‌ی تاریخ/وضعیت/متن</summary>
    public async Task<List<Queue>> SearchAsync(DateTime? from, DateTime? to, QueueStatus? status, string? search, int? companyId)
    {
        var query = _context.Queues
            .AsNoTracking()
            .Where(x => companyId == null || x.CompanyId == companyId);

        if (from.HasValue)
            query = query.Where(x => x.QueueDate.Date >= from.Value.Date);

        if (to.HasValue)
            query = query.Where(x => x.QueueDate.Date <= to.Value.Date);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.WaybillNumber.Contains(search) ||
                x.DriverName.Contains(search) ||
                x.DriverCarNumber.Contains(search) ||
                x.QueueNumber.Contains(search));
        }

        return await query
            .OrderByDescending(x => x.QueueDate)
            .ThenBy(x => x.QueueTime)
            .ToListAsync();
    }
}