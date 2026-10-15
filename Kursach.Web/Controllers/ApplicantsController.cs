using Kursach.Web.Models.Entities;
using Kursach.Web.Services;
using Kursach.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kursach.Web.Controllers;

/// <summary>
/// CRUD для соискателей и подбор вакансий.
/// </summary>
public class ApplicantsController : AgencyControllerBase
{
    private readonly IApplicantService _service;
    private readonly ISkillService _skillService;
    private readonly IVacancyService _vacancyService;

    /// <summary>
    /// Создаёт экземпляр контроллера.
    /// </summary>
    public ApplicantsController(
        IApplicantService service,
        ISkillService skillService,
        IVacancyService vacancyService)
    {
        _service = service;
        _skillService = skillService;
        _vacancyService = vacancyService;
    }

    /// <summary>Список соискателей.</summary>
    public async Task<IActionResult> Index(ApplicantFilterParams filter, string? reset, CancellationToken ct)
    {
        filter = ResolveFilter(filter, reset);
        var model = await _service.GetPagedAsync(filter, ct);
        ViewBag.Filter = filter;
        return View(model);
    }

    /// <summary>Карточка соискателя.</summary>
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
        var model = new ApplicantEditViewModel();
        await PopulateEditLookupsAsync(model, ct);
        return View(model);
    }

    /// <summary>Создание соискателя.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        ApplicantEditViewModel model,
        int[]? selectedSkillIds,
        CancellationToken ct)
    {
        model.Skills = BindApplicantSkills(selectedSkillIds);
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

    /// <summary>Обновление соискателя.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        ApplicantEditViewModel model,
        int[]? selectedSkillIds,
        CancellationToken ct)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        model.Skills = BindApplicantSkills(selectedSkillIds);
        if (!ModelState.IsValid)
        {
            await PopulateEditLookupsAsync(model, ct);
            return View(model);
        }

        await _service.UpdateAsync(model, ct);
        return RedirectToReturnOr(nameof(Index));
    }

    /// <summary>Подбор вакансий для соискателя.</summary>
    public async Task<IActionResult> MatchVacancies(int id, CancellationToken ct)
    {
        var applicant = await _service.GetByIdAsync(id, ct);
        if (applicant is null)
        {
            return NotFound();
        }

        var matches = await _vacancyService.MatchForApplicantAsync(id, ct);
        ViewBag.Applicant = applicant;
        return View(matches);
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

    /// <summary>Удаление соискателя.</summary>
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

    private List<ApplicantSkillItem> BindApplicantSkills(int[]? selectedSkillIds) =>
        (selectedSkillIds ?? []).Select(skillId => new ApplicantSkillItem
        {
            SkillId = skillId,
            Level = int.TryParse(Request.Form[$"level_{skillId}"], out var level) ? level : 3
        }).ToList();

    private static ApplicantEditViewModel MapToEditViewModel(Applicant entity) =>
        new()
        {
            Id = entity.Id,
            FullName = entity.FullName,
            Education = entity.Education,
            ExperienceYears = entity.ExperienceYears,
            DesiredSalary = entity.DesiredSalary,
            Skills = entity.ApplicantSkills
                .Select(s => new ApplicantSkillItem { SkillId = s.SkillId, Level = s.Level })
                .ToList()
        };

    private async Task PopulateEditLookupsAsync(ApplicantEditViewModel model, CancellationToken ct)
    {
        model.AvailableSkills = await _skillService.GetAllAsync(ct);
    }
}
