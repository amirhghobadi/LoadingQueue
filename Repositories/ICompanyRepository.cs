using LoadingQueue.Data;
using LoadingQueue.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadingQueue.Repositories;

public interface ICompanyRepository
{
    Task<List<Company>> GetAllAsync();
    Task<Company?> GetByIdAsync(int id);
    Task AddAsync(Company company);
    Task UpdateAsync(Company company);
    Task ToggleActiveAsync(int id);
    Task<bool> ExistsByNameAsync(string name, int? excludeId);
}

public class CompanyRepository : ICompanyRepository
{
    private readonly QueueDBContext _context;

    public CompanyRepository(QueueDBContext context)
    {
        _context = context;
    }

    public async Task<List<Company>> GetAllAsync()
    {
        return await _context.Companies
            .AsNoTracking()
            .Include(x => x.Drivers)
            .Include(x => x.Queues)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Company?> GetByIdAsync(int id)
    {
        return await _context.Companies.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Company company)
    {
        company.CreatedAt = DateTime.Now;
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Company company)
    {
        _context.Companies.Update(company);
        await _context.SaveChangesAsync();
    }

    /// <summary>غیرفعال/فعال کردن شرکت (به‌جای حذف واقعی، چون رانندگان/نوبت‌هاش وابسته‌ن)</summary>
    public async Task ToggleActiveAsync(int id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null) return;
        company.IsActive = !company.IsActive;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId)
    {
        return await _context.Companies.AnyAsync(x => x.Name == name && x.Id != excludeId);
    }
}