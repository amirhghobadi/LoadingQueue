using LoadingQueue.Data;
using LoadingQueue.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadingQueue.Repositories;

public interface ISettingsRepository
{
    Task<CompanySettings> GetOrCreateAsync(int companyId);
    Task UpdateAsync(CompanySettings settings);
}

public class SettingsRepository : ISettingsRepository
{
    private readonly QueueDBContext _context;

    public SettingsRepository(QueueDBContext context)
    {
        _context = context;
    }

    public async Task<CompanySettings> GetOrCreateAsync(int companyId)
    {
        // AsNoTracking مهمه: این متد گاهی فقط برای خوندن Id صدا زده
        // می‌شه (مثلاً تو SettingsController.Index موقع Update)، و اگه
        // بدون AsNoTracking باشه، EF این رکورد رو Track می‌کنه و بعداً
        // موقع Update کردن یه شیء دیگه با همون Id، تناقض پیش میاد.
        var settings = await _context.CompanySettingsList
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId);

        if (settings == null)
        {
            settings = new CompanySettings { CompanyId = companyId };
            _context.CompanySettingsList.Add(settings);
            await _context.SaveChangesAsync();
        }

        return settings;
    }

    public async Task UpdateAsync(CompanySettings settings)
    {
        // به‌جای Update() که کل گراف رو دستکاری می‌کنه، مستقیم وضعیتش
        // رو Modified می‌کنیم - چون مطمئنیم هیچ نمونه‌ی دیگه‌ای از این
        // رکورد الان Track نشده (به لطف AsNoTracking بالا)
        _context.Entry(settings).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}