using Kursach.Web.Data;
using Kursach.Web.Models.Entities;
using Kursach.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Kursach.Web.Services;

/// <summary>
/// Реализация сервиса соискателей.
/// </summary>
public class ApplicantService : IApplicantService
{
    private readonly AgencyDbContext _db;

    /// <summary>
    /// Создаёт экземпляр сервиса.
    /// </summary>
    public ApplicantService(AgencyDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<PagedListViewModel<Applicant>> GetPagedAsync(ApplicantFilterParams filter, CancellationToken cancellationToken = default)
    {
        filter.Normalize();

        var query = _db.Applicants.AsNoTracking()
            .Include(a => a.ApplicantSkills)
            .ThenInclude(s => s.Skill)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(a => a.FullName.Contains(term));
        }

        if (filter.Education.HasValue)
        {
            query = query.Where(a => a.Education == filter.Education.Value);
        }

        if (filter.SalaryFrom.HasValue)
        {
            query = query.Where(a => a.DesiredSalary >= filter.SalaryFrom.Value);
        }

        if (filter.SalaryTo.HasValue)
        {
            query = query.Where(a => a.DesiredSalary <= filter.SalaryTo.Value);
        }

        query = ApplySort(query, filter);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedListViewModel<Applicant>
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
    public Task<Applicant?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Applicants.AsNoTracking()
            .Include(a => a.ApplicantSkills)
            .ThenInclude(s => s.Skill)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Applicant>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _db.Applicants.AsNoTracking()
            .OrderBy(a => a.FullName)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<int> CreateAsync(ApplicantEditViewModel model, CancellationToken cancellationToken = default)
    {
        var applicant = new Applicant
        {
            FullName = model.FullName.Trim(),
            Education = model.Education,
            ExperienceYears = model.ExperienceYears,
            DesiredSalary = model.DesiredSalary
        };

        _db.Applicants.Add(applicant);
        await _db.SaveChangesAsync(cancellationToken);
        await ReplaceSkillsAsync(applicant.Id, model.Skills, cancellationToken);
        return applicant.Id;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(ApplicantEditViewModel model, CancellationToken cancellationToken = default)
    {
        var applicant = await _db.Applicants
            .Include(a => a.ApplicantSkills)
            .FirstOrDefaultAsync(a => a.Id == model.Id, cancellationToken);

        if (applicant is null)
        {
            throw new InvalidOperationException("Соискатель не найден.");
        }

        applicant.FullName = model.FullName.Trim();
        applicant.Education = model.Education;
        applicant.ExperienceYears = model.ExperienceYears;
        applicant.DesiredSalary = model.DesiredSalary;

        _db.ApplicantSkills.RemoveRange(applicant.ApplicantSkills);
        applicant.ApplicantSkills.Clear();
        AddSkills(applicant, model.Skills);

        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Applicants.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            throw new InvalidOperationException("Соискатель не найден.");
        }

        _db.Applicants.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task ReplaceSkillsAsync(int applicantId, IEnumerable<ApplicantSkillItem> skills, CancellationToken cancellationToken)
    {
        var existing = await _db.ApplicantSkills.Where(s => s.ApplicantId == applicantId).ToListAsync(cancellationToken);
        _db.ApplicantSkills.RemoveRange(existing);

        foreach (var item in NormalizeSkills(skills))
        {
            _db.ApplicantSkills.Add(new ApplicantSkill
            {
                ApplicantId = applicantId,
                SkillId = item.SkillId,
                Level = item.Level
            });
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    private static void AddSkills(Applicant applicant, IEnumerable<ApplicantSkillItem> skills)
    {
        foreach (var item in NormalizeSkills(skills))
        {
            applicant.ApplicantSkills.Add(new ApplicantSkill
            {
                SkillId = item.SkillId,
                Level = item.Level
            });
        }
    }

    private static IEnumerable<ApplicantSkillItem> NormalizeSkills(IEnumerable<ApplicantSkillItem> skills) =>
        skills.Where(s => s.SkillId > 0).GroupBy(s => s.SkillId).Select(g => g.First());

    private static IQueryable<Applicant> ApplySort(IQueryable<Applicant> query, FilterParams filter)
    {
        var desc = string.Equals(filter.SortDir, "desc", StringComparison.OrdinalIgnoreCase);
        return (filter.SortBy?.ToLowerInvariant()) switch
        {
            "education" => desc ? query.OrderByDescending(a => a.Education) : query.OrderBy(a => a.Education),
            "experience" or "experienceyears" => desc
                ? query.OrderByDescending(a => a.ExperienceYears)
                : query.OrderBy(a => a.ExperienceYears),
            "salary" or "desiredsalary" => desc
                ? query.OrderByDescending(a => a.DesiredSalary)
                : query.OrderBy(a => a.DesiredSalary),
            "id" => desc ? query.OrderByDescending(a => a.Id) : query.OrderBy(a => a.Id),
            _ => desc ? query.OrderByDescending(a => a.FullName) : query.OrderBy(a => a.FullName)
        };
    }
}
