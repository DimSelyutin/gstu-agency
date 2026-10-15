using Kursach.Web.Data;
using Kursach.Web.Models.Entities;
using Kursach.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Kursach.Web.Services;

/// <summary>
/// Реализация сервиса сфер деятельности.
/// </summary>
public class IndustryService : IIndustryService
{
    private readonly AgencyDbContext _db;

    /// <summary>
    /// Создаёт экземпляр сервиса.
    /// </summary>
    public IndustryService(AgencyDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<PagedListViewModel<Industry>> GetPagedAsync(FilterParams filter, CancellationToken cancellationToken = default)
    {
        filter.Normalize();

        var query = _db.Industries.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(i => i.Name.Contains(term));
        }

        query = ApplySort(query, filter);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedListViewModel<Industry>
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
    public Task<Industry?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Industries.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Industry>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _db.Industries.AsNoTracking().OrderBy(i => i.Name).ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<int> CreateAsync(Industry entity, CancellationToken cancellationToken = default)
    {
        await EnsureUniqueNameAsync(entity.Name, excludeId: null, cancellationToken);
        _db.Industries.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Industry entity, CancellationToken cancellationToken = default)
    {
        await EnsureUniqueNameAsync(entity.Name, entity.Id, cancellationToken);
        _db.Industries.Update(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Industries.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            throw new InvalidOperationException("Сфера деятельности не найдена.");
        }

        _db.Industries.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureUniqueNameAsync(string name, int? excludeId, CancellationToken cancellationToken)
    {
        var normalized = name.Trim();
        var query = _db.Industries.AsNoTracking().Where(i => i.Name.ToLower() == normalized.ToLower());
        if (excludeId.HasValue)
        {
            query = query.Where(i => i.Id != excludeId.Value);
        }

        if (await query.AnyAsync(cancellationToken))
        {
            throw new InvalidOperationException("Сфера деятельности с таким названием уже существует.");
        }
    }

    private static IQueryable<Industry> ApplySort(IQueryable<Industry> query, FilterParams filter)
    {
        var desc = string.Equals(filter.SortDir, "desc", StringComparison.OrdinalIgnoreCase);
        return (filter.SortBy?.ToLowerInvariant()) switch
        {
            "id" => desc ? query.OrderByDescending(i => i.Id) : query.OrderBy(i => i.Id),
            _ => desc ? query.OrderByDescending(i => i.Name) : query.OrderBy(i => i.Name)
        };
    }
}
