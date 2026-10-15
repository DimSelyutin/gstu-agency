using Kursach.Web.Data;
using Kursach.Web.Models.Entities;
using Kursach.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Kursach.Web.Services;

/// <summary>
/// Реализация сервиса собеседований.
/// </summary>
public class InterviewService : IInterviewService
{
    private readonly AgencyDbContext _db;

    /// <summary>
    /// Создаёт экземпляр сервиса.
    /// </summary>
    public InterviewService(AgencyDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<PagedListViewModel<Interview>> GetPagedAsync(InterviewFilterParams filter, CancellationToken cancellationToken = default)
    {
        filter.Normalize();

        var query = BuildQuery();

        if (filter.DateFrom.HasValue)
        {
            query = query.Where(i => i.Date >= filter.DateFrom.Value);
        }

        if (filter.DateTo.HasValue)
        {
            query = query.Where(i => i.Date <= filter.DateTo.Value);
        }

        if (filter.ApplicantId.HasValue)
        {
            query = query.Where(i => i.ApplicantId == filter.ApplicantId.Value);
        }

        if (filter.VacancyId.HasValue)
        {
            query = query.Where(i => i.VacancyId == filter.VacancyId.Value);
        }

        if (filter.Result.HasValue)
        {
            query = query.Where(i => i.Result == filter.Result.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(i =>
                (i.Applicant != null && i.Applicant.FullName.Contains(term)) ||
                (i.Vacancy != null && i.Vacancy.Title.Contains(term)));
        }

        query = ApplySort(query, filter);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedListViewModel<Interview>
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
    public Task<Interview?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        BuildQuery().FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<int> CreateAsync(Interview entity, CancellationToken cancellationToken = default)
    {
        var vacancy = await _db.Vacancies.AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == entity.VacancyId, cancellationToken);

        if (vacancy is null)
        {
            throw new InvalidOperationException("Вакансия не найдена.");
        }

        if (vacancy.Status == VacancyStatus.Closed)
        {
            throw new InvalidOperationException("Нельзя назначить собеседование по закрытой вакансии.");
        }

        _db.Interviews.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Interview entity, CancellationToken cancellationToken = default)
    {
        _db.Interviews.Update(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Interviews.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            throw new InvalidOperationException("Собеседование не найдено.");
        }

        _db.Interviews.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Interview> BuildQuery() =>
        _db.Interviews.AsNoTracking()
            .Include(i => i.Applicant)
            .Include(i => i.Vacancy!)
            .ThenInclude(v => v.Company);

    private static IQueryable<Interview> ApplySort(IQueryable<Interview> query, FilterParams filter)
    {
        var desc = string.Equals(filter.SortDir, "desc", StringComparison.OrdinalIgnoreCase);
        return (filter.SortBy?.ToLowerInvariant()) switch
        {
            "result" => desc ? query.OrderByDescending(i => i.Result) : query.OrderBy(i => i.Result),
            "applicant" => desc
                ? query.OrderByDescending(i => i.Applicant!.FullName)
                : query.OrderBy(i => i.Applicant!.FullName),
            "vacancy" => desc
                ? query.OrderByDescending(i => i.Vacancy!.Title)
                : query.OrderBy(i => i.Vacancy!.Title),
            "id" => desc ? query.OrderByDescending(i => i.Id) : query.OrderBy(i => i.Id),
            _ => desc ? query.OrderByDescending(i => i.Date) : query.OrderBy(i => i.Date)
        };
    }
}
