using LoadingQueue.Data;
using LoadingQueue.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadingQueue.Repositories;

public interface IShippingCompanyRepository
{
    Task<List<ShippingCompany>> GetAllAsync(int? companyId);
    Task<ShippingCompany?> GetByIdAsync(int id, int? companyId);
    Task<List<string>> GetActiveNamesAsync(int? companyId);
    Task AddAsync(ShippingCompany shippingCompany);
    Task UpdateAsync(ShippingCompany shippingCompany);
    Task<bool> ExistsByNameAsync(string name, int companyId, int? excludeId);
}

public class ShippingCompanyRepository : IShippingCompanyRepository
{
    private readonly QueueDBContext _context;

    public ShippingCompanyRepository(QueueDBContext context)
    {
        _context = context;
    }

    public async Task<List<ShippingCompany>> GetAllAsync(int? companyId)
    {
        return await _context.ShippingCompanies
            .AsNoTracking()
            .Where(x => companyId == null || x.CompanyId == companyId)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<ShippingCompany?> GetByIdAsync(int id, int? companyId)
    {
        return await _context.ShippingCompanies
            .Where(x => companyId == null || x.CompanyId == companyId)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>فقط اسم باربری‌های فعال - برای dropdown صفحه‌ی صف بارگیری</summary>
    public async Task<List<string>> GetActiveNamesAsync(int? companyId)
    {
        return await _context.ShippingCompanies
            .AsNoTracking()
            .Where(x => companyId == null || x.CompanyId == companyId)
            .Where(x => x.Status == ShippingCompanyStatus.Active)
            .OrderBy(x => x.Name)
            .Select(x => x.Name)
            .ToListAsync();
    }

    public async Task AddAsync(ShippingCompany shippingCompany)
    {
        shippingCompany.CreatedAt = DateTime.Now;
        _context.ShippingCompanies.Add(shippingCompany);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ShippingCompany shippingCompany)
    {
        _context.ShippingCompanies.Update(shippingCompany);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name, int companyId, int? excludeId)
    {
        return await _context.ShippingCompanies
            .AnyAsync(x => x.CompanyId == companyId && x.Name == name && x.Id != excludeId);
    }
}