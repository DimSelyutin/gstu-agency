using Kursach.Web.Data;
using Kursach.Web.Models.Entities;
using Kursach.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Kursach.Web.Services;

/// <summary>
/// Реализация сервиса вакансий.
/// </summary>
public class VacancyService : IVacancyService
{
    private readonly AgencyDbContext _db;

    /// <summary>
    /// Создаёт экземпляр сервиса.
    /// </summary>
    public VacancyService(AgencyDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<PagedListViewModel<Vacancy>> GetPagedAsync(VacancyFilterParams filter, CancellationToken cancellationToken = default)
    {
        filter.Normalize();

        var query = BuildVacancyQuery();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(v => v.Title.Contains(term));
        }

        if (filter.IndustryId.HasValue)
        {
            query = query.Where(v => v.Company!.IndustryId == filter.IndustryId.Value);
        }

        if (filter.CompanyId.HasValue)
        {
            query = query.Where(v => v.CompanyId == filter.CompanyId.Value);
        }

        if (filter.SalaryFrom.HasValue)
        {
            query = query.Where(v => v.Salary >= filter.SalaryFrom.Value);
        }

        if (filter.SalaryTo.HasValue)
        {
            query = query.Where(v => v.Salary <= filter.SalaryTo.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(v => v.Status == filter.Status.Value);
        }

        query = ApplySort(query, filter);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedListViewModel<Vacancy>
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
    public Task<Vacancy?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        BuildVacancyQuery().FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Vacancy>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await BuildVacancyQuery().OrderBy(v => v.Title).ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<int> CreateAsync(VacancyEditViewModel model, CancellationToken cancellationToken = default)
    {
        var vacancy = new Vacancy
        {
            Title = model.Title.Trim(),
            CompanyId = model.CompanyId,
            Salary = model.Salary,
            Description = model.Description,
            Status = model.Status,
            ClosedDate = model.ClosedDate
        };

        _db.Vacancies.Add(vacancy);
        await _db.SaveChangesAsync(cancellationToken);
        await ReplaceSkillsAsync(vacancy.Id, model.Skills, cancellationToken);
        return vacancy.Id;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(VacancyEditViewModel model, CancellationToken cancellationToken = default)
    {
        var vacancy = await _db.Vacancies
            .Include(v => v.VacancySkills)
            .FirstOrDefaultAsync(v => v.Id == model.Id, cancellationToken);

        if (vacancy is null)
        {
            throw new InvalidOperationException("Вакансия не найдена.");
        }

        vacancy.Title = model.Title.Trim();
        vacancy.CompanyId = model.CompanyId;
        vacancy.Salary = model.Salary;
        vacancy.Description = model.Description;
        vacancy.Status = model.Status;
        vacancy.ClosedDate = model.ClosedDate;

        _db.VacancySkills.RemoveRange(vacancy.VacancySkills);
        vacancy.VacancySkills.Clear();
        AddSkills(vacancy, model.Skills);

        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Vacancies.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            throw new InvalidOperationException("Вакансия не найдена.");
        }

        _db.Vacancies.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<VacancyMatchViewModel>> MatchForApplicantAsync(int applicantId, CancellationToken cancellationToken = default)
    {
        var applicantExists = await _db.Applicants.AsNoTracking().AnyAsync(a => a.Id == applicantId, cancellationToken);
        if (!applicantExists)
        {
            throw new InvalidOperationException("Соискатель не найден.");
        }

        var applicantSkills = await _db.ApplicantSkills.AsNoTracking()
            .Where(s => s.ApplicantId == applicantId)
            .ToDictionaryAsync(s => s.SkillId, s => s.Level, cancellationToken);

        var vacancies = await _db.Vacancies.AsNoTracking()
            .Where(v => v.Status == VacancyStatus.Open)
            .Include(v => v.Company!)
            .ThenInclude(c => c.Industry)
            .Include(v => v.VacancySkills)
            .ToListAsync(cancellationToken);

        var matches = vacancies.Select(v =>
        {
            var required = v.VacancySkills.Count;
            var matched = required == 0
                ? 0
                : v.VacancySkills.Count(vs =>
                    applicantSkills.TryGetValue(vs.SkillId, out var level) && level >= vs.MinLevel);

            var ratio = required == 0 ? 1d : matched / (double)required;

            return new VacancyMatchViewModel
            {
                VacancyId = v.Id,
                Title = v.Title,
                CompanyName = v.Company?.Name ?? string.Empty,
                Salary = v.Salary,
                IndustryName = v.Company?.Industry?.Name ?? string.Empty,
                MatchedSkills = matched,
                RequiredSkills = required,
                MatchRatio = ratio
            };
        })
        .OrderByDescending(m => m.MatchRatio)
        .ThenByDescending(m => m.Salary)
        .ToList();

        return matches;
    }

    /// <inheritdoc />
    public async Task<PagedListViewModel<Vacancy>> GetOpenByIndustryAsync(int? industryId, FilterParams filter, CancellationToken cancellationToken = default)
    {
        filter.Normalize();

        var query = BuildVacancyQuery().Where(v => v.Status == VacancyStatus.Open);

        if (industryId.HasValue)
        {
            query = query.Where(v => v.Company!.IndustryId == industryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(v => v.Title.Contains(term));
        }

        query = ApplySort(query, filter);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedListViewModel<Vacancy>
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
    public async Task<ClosedVacancyReportViewModel> GetClosedReportAsync(DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default)
    {
        var query = _db.Vacancies.AsNoTracking()
            .Where(v => v.Status == VacancyStatus.Closed && v.ClosedDate != null)
            .Include(v => v.Company)
            .Include(v => v.Interviews)
            .ThenInclude(i => i.Applicant)
            .AsQueryable();

        if (from.HasValue)
        {
            query = query.Where(v => v.ClosedDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(v => v.ClosedDate <= to.Value);
        }

        var vacancies = await query.OrderByDescending(v => v.ClosedDate).ToListAsync(cancellationToken);

        var items = vacancies.Select(v =>
        {
            var hired = v.Interviews.FirstOrDefault(i => i.Result == InterviewResult.Hired);
            return new ClosedVacancyReportItem
            {
                VacancyId = v.Id,
                Title = v.Title,
                CompanyName = v.Company?.Name ?? string.Empty,
                HiredApplicantName = hired?.Applicant?.FullName ?? string.Empty,
                ClosedDate = v.ClosedDate!.Value,
                Salary = v.Salary
            };
        }).ToList();

        var totalSalary = items.Sum(i => i.Salary);
        var count = items.Count;

        return new ClosedVacancyReportViewModel
        {
            Items = items,
            From = from,
            To = to,
            TotalCount = count,
            TotalSalary = totalSalary,
            AverageSalary = count == 0 ? 0 : totalSalary / count
        };
    }

    /// <inheritdoc />
    public async Task CloseAsync(int vacancyId, int hiredInterviewId, CancellationToken cancellationToken = default)
    {
        var vacancy = await _db.Vacancies.FindAsync([vacancyId], cancellationToken);
        if (vacancy is null)
        {
            throw new InvalidOperationException("Вакансия не найдена.");
        }

        var interview = await _db.Interviews
            .FirstOrDefaultAsync(i => i.Id == hiredInterviewId && i.VacancyId == vacancyId, cancellationToken);

        if (interview is null)
        {
            throw new InvalidOperationException("Собеседование не найдено.");
        }

        interview.Result = InterviewResult.Hired;
        vacancy.Status = VacancyStatus.Closed;
        vacancy.ClosedDate = DateOnly.FromDateTime(DateTime.Today);

        await _db.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Vacancy> BuildVacancyQuery() =>
        _db.Vacancies.AsNoTracking()
            .Include(v => v.Company!)
            .ThenInclude(c => c.Industry)
            .Include(v => v.VacancySkills)
            .ThenInclude(vs => vs.Skill);

    private async Task ReplaceSkillsAsync(int vacancyId, IEnumerable<VacancySkillItem> skills, CancellationToken cancellationToken)
    {
        var existing = await _db.VacancySkills.Where(s => s.VacancyId == vacancyId).ToListAsync(cancellationToken);
        _db.VacancySkills.RemoveRange(existing);

        foreach (var item in NormalizeSkills(skills))
        {
            _db.VacancySkills.Add(new VacancySkill
            {
                VacancyId = vacancyId,
                SkillId = item.SkillId,
                MinLevel = item.MinLevel
            });
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    private static void AddSkills(Vacancy vacancy, IEnumerable<VacancySkillItem> skills)
    {
        foreach (var item in NormalizeSkills(skills))
        {
            vacancy.VacancySkills.Add(new VacancySkill
            {
                SkillId = item.SkillId,
                MinLevel = item.MinLevel
            });
        }
    }

    private static IEnumerable<VacancySkillItem> NormalizeSkills(IEnumerable<VacancySkillItem> skills) =>
        skills.Where(s => s.SkillId > 0).GroupBy(s => s.SkillId).Select(g => g.First());

    private static IQueryable<Vacancy> ApplySort(IQueryable<Vacancy> query, FilterParams filter)
    {
        var desc = string.Equals(filter.SortDir, "desc", StringComparison.OrdinalIgnoreCase);
        return (filter.SortBy?.ToLowerInvariant()) switch
        {
            "salary" => desc ? query.OrderByDescending(v => v.Salary) : query.OrderBy(v => v.Salary),
            "status" => desc ? query.OrderByDescending(v => v.Status) : query.OrderBy(v => v.Status),
            "company" => desc
                ? query.OrderByDescending(v => v.Company!.Name)
                : query.OrderBy(v => v.Company!.Name),
            "id" => desc ? query.OrderByDescending(v => v.Id) : query.OrderBy(v => v.Id),
            _ => desc ? query.OrderByDescending(v => v.Title) : query.OrderBy(v => v.Title)
        };
    }
}
