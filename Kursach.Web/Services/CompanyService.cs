using Kursach.Web.Data;
using Kursach.Web.Models.Entities;
using Kursach.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Kursach.Web.Services;

/// <summary>
/// Реализация сервиса компаний.
/// </summary>
public class CompanyService : ICompanyService
{
    private readonly AgencyDbContext _db;

    /// <summary>
    /// Создаёт экземпляр сервиса.
    /// </summary>
    public CompanyService(AgencyDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<PagedListViewModel<Company>> GetPagedAsync(CompanyFilterParams filter, CancellationToken cancellationToken = default)
    {
        filter.Normalize();

        var query = _db.Companies.AsNoTracking().Include(c => c.Industry).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(c => c.Name.Contains(term));
        }

        if (filter.IndustryId.HasValue)
        {
            query = query.Where(c => c.IndustryId == filter.IndustryId.Value);
        }

        query = ApplySort(query, filter);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedListViewModel<Company>
        {
            Items = items,
            PageNumber = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = totalCount,
            SortBy = filter.SortBy,
            SortDir = filter.SortDir
        };
    }

    /// <inheritdoc />
    public Task<Company?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Companies.AsNoTracking()
            .Include(c => c.Industry)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Company>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _db.Companies.AsNoTracking()
            .Include(c => c.Industry)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<int> CreateAsync(Company entity, CancellationToken cancellationToken = default)
    {
        _db.Companies.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Company entity, CancellationToken cancellationToken = default)
    {
        _db.Companies.Update(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Companies.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            throw new InvalidOperationException("Компания не найдена.");
        }

        _db.Companies.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Company> ApplySort(IQueryable<Company> query, FilterParams filter)
    {
        var desc = string.Equals(filter.SortDir, "desc", StringComparison.OrdinalIgnoreCase);
        return (filter.SortBy?.ToLowerInvariant()) switch
        {
            "industry" => desc
                ? query.OrderByDescending(c => c.Industry!.Name)
                : query.OrderBy(c => c.Industry!.Name),
            "id" => desc ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
            _ => desc ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name)
        };
    }
}
