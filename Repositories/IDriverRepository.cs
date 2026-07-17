using LoadingQueue.Data;
using LoadingQueue.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadingQueue.Repositories;

public interface IDriverRepository
{
    // پارامتر companyId: null یعنی بدون محدودیت (SystemAdmin)،
    // عدد یعنی فقط رانندگان همون شرکت
    Task<List<Driver>> GetAllAsync(int? companyId);
    Task<Driver?> GetByIdAsync(int id, int? companyId);
    Task AddAsync(Driver driver);
    Task UpdateAsync(Driver driver);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(Driver driver);
    Task<List<Driver>> SearchByCarNumber(string carNumber, int? companyId);
}

public class DriverRepository : IDriverRepository
{
    private readonly QueueDBContext _context;

    public DriverRepository(QueueDBContext context)
    {
        _context = context;
    }

    /// <summary>لیست همه‌ی رانندگان فعال، به همراه تعداد نوبت هرکدوم</summary>
    public async Task<List<Driver>> GetAllAsync(int? companyId)
    {
        return await _context.Drivers
            .AsNoTracking()
            .Include(x => x.Queues)
            .Where(x => companyId == null || x.CompanyId == companyId)
            .OrderBy(x => x.FullName)
            .ToListAsync();
    }

    /// <summary>
    /// گرفتن یک راننده با Id. از IgnoreQueryFilters استفاده می‌کنه چون
    /// این متد در صفحه‌ی Edit/Details هم استفاده می‌شه و باید حتی
    /// راننده‌های حذف‌شده (Soft Delete) هم قابل مشاهده باشن.
    /// </summary>
    public async Task<Driver?> GetByIdAsync(int id, int? companyId)
    {
        return await _context.Drivers
            .IgnoreQueryFilters()
            .Include(x => x.Queues)
            .Where(x => companyId == null || x.CompanyId == companyId)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Driver driver)
    {
        driver.RegisterDate = DateTime.Now;

        _context.Drivers.Add(driver);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Driver driver)
    {
        _context.Drivers.Update(driver);

        await _context.SaveChangesAsync();
    }

    /// <summary>حذف نرم: راننده واقعاً از دیتابیس پاک نمی‌شه، فقط IsDeleted=true می‌شه</summary>
    public async Task DeleteAsync(int id)
    {
        var driver = await _context.Drivers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (driver == null)
            return;

        driver.IsDeleted = true;
        driver.DeletedAt = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    /// <summary>چک می‌کنه آیا شماره ماشین قبلاً برای یه راننده‌ی دیگه ثبت شده یا نه</summary>
    public async Task<bool> ExistsAsync(Driver driver)
    {
        return await _context.Drivers
            .IgnoreQueryFilters()
            .AnyAsync(x =>
                x.Id != driver.Id &&
                x.CarNumber == driver.CarNumber);
    }

    /// <summary>جستجوی راننده بر اساس بخشی از شماره ماشین (برای بخش صف بارگیری)</summary>
    public async Task<List<Driver>> SearchByCarNumber(string carNumber, int? companyId)
    {
        if (string.IsNullOrWhiteSpace(carNumber))
            return new List<Driver>();

        carNumber = carNumber.Trim();

        return await _context.Drivers
            .AsNoTracking()
            .Where(x => x.CarNumber.Contains(carNumber))
            .Where(x => companyId == null || x.CompanyId == companyId)
            .OrderBy(x => x.CarNumber)
            .ThenBy(x => x.FullName)
            .Take(20)
            .ToListAsync();
    }
}