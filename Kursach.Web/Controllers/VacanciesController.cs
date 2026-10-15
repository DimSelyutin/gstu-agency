using Kursach.Web.Data;
using Kursach.Web.Models.Entities;
using Kursach.Web.Services;
using Kursach.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kursach.Web.Controllers;

/// <summary>
/// CRUD для вакансий, отчёты и закрытие.
/// </summary>
public class VacanciesController : AgencyControllerBase
{
    private readonly IVacancyService _service;
    private readonly ISkillService _skillService;
    private readonly ICompanyService _companyService;
    private readonly IIndustryService _industryService;
    private readonly AgencyDbContext _db;

    /// <summary>
    /// Создаёт экземпляр контроллера.
    /// </summary>
    public VacanciesController(
        IVacancyService service,
        ISkillService skillService,
        ICompanyService companyService,
        IIndustryService industryService,
        AgencyDbContext db)
    {
        _service = service;
        _skillService = skillService;
        _companyService = companyService;
        _industryService = industryService;
        _db = db;
    }

    /// <summary>Список вакансий.</summary>
    public async Task<IActionResult> Index(VacancyFilterParams filter, string? reset, CancellationToken ct)
    {
        filter = ResolveFilter(filter, reset);
        await PopulateIndexLookupsAsync(filter, ct);
        var model = await _service.GetPagedAsync(filter, ct);
        ViewBag.Filter = filter;
        return View(model);
    }

    /// <summary>Карточка вакансии.</summary>
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var entity = await _service.GetByIdAsync(id, ct);
        if (entity is null)
        {
            return NotFound();
        }

        return View(entity);
    }

    /// <summary>Форма создания.</summary>
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        RememberReturnUrl();
        var model = new VacancyEditViewModel();
        await PopulateEditLookupsAsync(model, ct);
        return View(model);
    }

    /// <summary>Создание вакансии.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        VacancyEditViewModel model,
        int[]? selectedSkillIds,
        CancellationToken ct)
    {
        model.Skills = BindVacancySkills(selectedSkillIds);
        if (!ModelState.IsValid)
        {
            await PopulateEditLookupsAsync(model, ct);
            return View(model);
        }

        await _service.CreateAsync(model, ct);
        return RedirectToReturnOr(nameof(Index));
    }

    /// <summary>Форма редактирования.</summary>
    public async Task<IActionResult> Edit(int id, string? returnUrl, CancellationToken ct)
    {
        RememberReturnUrl(returnUrl);
        var entity = await _service.GetByIdAsync(id, ct);
        if (entity is null)
        {
            return NotFound();
        }

        var model = MapToEditViewModel(entity);
        await PopulateEditLookupsAsync(model, ct);
        return View(model);
    }

    /// <summary>Обновление вакансии.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        VacancyEditViewModel model,
        int[]? selectedSkillIds,
        CancellationToken ct)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        model.Skills = BindVacancySkills(selectedSkillIds);
        if (!ModelState.IsValid)
        {
            await PopulateEditLookupsAsync(model, ct);
            return View(model);
        }

        await _service.UpdateAsync(model, ct);
        return RedirectToReturnOr(nameof(Index));
    }

    /// <summary>Открытые вакансии по сфере.</summary>
    public async Task<IActionResult> OpenByIndustry(int? industryId, FilterParams filter, string? reset, CancellationToken ct)
    {
        filter = ResolveFilter(filter, reset);
        var industries = await _industryService.GetAllAsync(ct);
        ViewBag.IndustryId = new SelectList(industries, nameof(Industry.Id), nameof(Industry.Name), industryId);
        ViewBag.SelectedIndustryId = industryId;
        ViewBag.Filter = filter;
        var model = await _service.GetOpenByIndustryAsync(industryId, filter, ct);
        return View(model);
    }

    /// <summary>Отчёт по закрытым вакансиям.</summary>
    public async Task<IActionResult> ClosedReport(DateOnly? from, DateOnly? to, CancellationToken ct)
    {
        var model = await _service.GetClosedReportAsync(from, to, ct);
        return View(model);
    }

    /// <summary>Форма закрытия вакансии.</summary>
    public async Task<IActionResult> Close(int id, CancellationToken ct)
    {
        var vacancy = await _service.GetByIdAsync(id, ct);
        if (vacancy is null)
        {
            return NotFound();
        }

        if (vacancy.Status == VacancyStatus.Closed)
        {
            return RedirectToAction(nameof(Details), new { id });
        }

        var model = new VacancyCloseViewModel
        {
            VacancyId = vacancy.Id,
            Title = vacancy.Title
        };

        await PopulateCloseInterviewSelectListAsync(id, null, ct);
        return View(model);
    }

    /// <summary>Закрытие вакансии.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(int id, VacancyCloseViewModel model, CancellationToken ct)
    {
        if (id != model.VacancyId)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await PopulateCloseInterviewSelectListAsync(id, model.HiredInterviewId, ct);
            return View(model);
        }

        try
        {
            await _service.CloseAsync(model.VacancyId, model.HiredInterviewId, ct);
            return RedirectToAction(nameof(Details), new { id = model.VacancyId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await PopulateCloseInterviewSelectListAsync(id, model.HiredInterviewId, ct);
            return View(model);
        }
    }

    /// <summary>Подтверждение удаления.</summary>
    public async Task<IActionResult> Delete(int id, string? returnUrl, CancellationToken ct)
    {
        RememberReturnUrl(returnUrl);
        var entity = await _service.GetByIdAsync(id, ct);
        if (entity is null)
        {
            return NotFound();
        }

        return View(entity);
    }

    /// <summary>Удаление вакансии.</summary>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        try
        {
            await _service.DeleteAsync(id, ct);
            return RedirectToReturnOr(nameof(Index));
        }
        catch (DbUpdateException)
        {
            AddDeleteConstraintError();
            var entity = await _service.GetByIdAsync(id, ct);
            if (entity is null)
            {
                return NotFound();
            }

            return View(nameof(Delete), entity);
        }
    }

    private List<VacancySkillItem> BindVacancySkills(int[]? selectedSkillIds) =>
        (selectedSkillIds ?? []).Select(skillId => new VacancySkillItem
        {
            SkillId = skillId,
            MinLevel = int.TryParse(Request.Form[$"minLevel_{skillId}"], out var level) ? level : 3
        }).ToList();

    private static VacancyEditViewModel MapToEditViewModel(Vacancy entity) =>
        new()
        {
            Id = entity.Id,
            Title = entity.Title,
            CompanyId = entity.CompanyId,
            Salary = entity.Salary,
            Description = entity.Description,
            Status = entity.Status,
            ClosedDate = entity.ClosedDate,
            Skills = entity.VacancySkills
                .Select(s => new VacancySkillItem { SkillId = s.SkillId, MinLevel = s.MinLevel })
                .ToList()
        };

    private async Task PopulateEditLookupsAsync(VacancyEditViewModel model, CancellationToken ct)
    {
        model.AvailableSkills = await _skillService.GetAllAsync(ct);
        var companies = await _companyService.GetAllAsync(ct);
        ViewBag.CompanyId = new SelectList(companies, nameof(Company.Id), nameof(Company.Name), model.CompanyId);
    }

    private async Task PopulateIndexLookupsAsync(VacancyFilterParams filter, CancellationToken ct)
    {
        var industries = await _industryService.GetAllAsync(ct);
        ViewBag.IndustryId = new SelectList(industries, nameof(Industry.Id), nameof(Industry.Name), filter.IndustryId);
        var companies = await _companyService.GetAllAsync(ct);
        ViewBag.CompanyId = new SelectList(companies, nameof(Company.Id), nameof(Company.Name), filter.CompanyId);
        ViewBag.Status = new SelectList(
            Enum.GetValues<VacancyStatus>().Select(s => new { Value = (int)s, Text = s.ToString() }),
            "Value",
            "Text",
            filter.Status.HasValue ? (int)filter.Status.Value : null);
    }

    private async Task PopulateCloseInterviewSelectListAsync(int vacancyId, int? selectedId, CancellationToken ct)
    {
        var interviews = await _db.Interviews.AsNoTracking()
            .Include(i => i.Applicant)
            .Where(i => i.VacancyId == vacancyId)
            .Where(i => i.Result == InterviewResult.Passed || i.Result == InterviewResult.Scheduled)
            .OrderByDescending(i => i.Result == InterviewResult.Passed)
            .ThenBy(i => i.Date)
            .ToListAsync(ct);

        var items = interviews.Select(i => new
        {
            i.Id,
            Text = $"{i.Applicant?.FullName ?? "—"} — {i.Date:dd.MM.yyyy} ({i.Result})"
        });

        ViewBag.HiredInterviewId = new SelectList(items, "Id", "Text", selectedId);
    }
}
