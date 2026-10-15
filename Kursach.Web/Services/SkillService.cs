using Kursach.Web.Data;
using Kursach.Web.Models.Entities;
using Kursach.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Kursach.Web.Services;

/// <summary>
/// Реализация сервиса навыков.
/// </summary>
public class SkillService : ISkillService
{
    private readonly AgencyDbContext _db;

    /// <summary>
    /// Создаёт экземпляр сервиса.
    /// </summary>
    public SkillService(AgencyDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<PagedListViewModel<Skill>> GetPagedAsync(FilterParams filter, CancellationToken cancellationToken = default)
    {
        filter.Normalize();

        var query = _db.Skills.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(s => s.Name.Contains(term));
        }

        query = ApplySort(query, filter);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedListViewModel<Skill>
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
    public Task<Skill?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Skills.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Skill>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _db.Skills.AsNoTracking().OrderBy(s => s.Name).ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<int> CreateAsync(Skill entity, CancellationToken cancellationToken = default)
    {
        await EnsureUniqueNameAsync(entity.Name, excludeId: null, cancellationToken);
        _db.Skills.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Skill entity, CancellationToken cancellationToken = default)
    {
        await EnsureUniqueNameAsync(entity.Name, entity.Id, cancellationToken);
        _db.Skills.Update(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Skills.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            throw new InvalidOperationException("Навык не найден.");
        }

        _db.Skills.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureUniqueNameAsync(string name, int? excludeId, CancellationToken cancellationToken)
    {
        var normalized = name.Trim();
        var query = _db.Skills.AsNoTracking().Where(s => s.Name.ToLower() == normalized.ToLower());
        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }

        if (await query.AnyAsync(cancellationToken))
        {
            throw new InvalidOperationException("Навык с таким названием уже существует.");
        }
    }

    private static IQueryable<Skill> ApplySort(IQueryable<Skill> query, FilterParams filter)
    {
        var desc = string.Equals(filter.SortDir, "desc", StringComparison.OrdinalIgnoreCase);
        return (filter.SortBy?.ToLowerInvariant()) switch
        {
            "id" => desc ? query.OrderByDescending(s => s.Id) : query.OrderBy(s => s.Id),
            _ => desc ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name)
        };
    }
}
