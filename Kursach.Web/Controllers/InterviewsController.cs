using Kursach.Web.Models.Entities;
using Kursach.Web.Services;
using Kursach.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kursach.Web.Controllers;

/// <summary>
/// CRUD для собеседований.
/// </summary>
public class InterviewsController : AgencyControllerBase
{
    private readonly IInterviewService _service;
    private readonly IApplicantService _applicantService;
    private readonly IVacancyService _vacancyService;

    /// <summary>
    /// Создаёт экземпляр контроллера.
    /// </summary>
    public InterviewsController(
        IInterviewService service,
        IApplicantService applicantService,
        IVacancyService vacancyService)
    {
        _service = service;
        _applicantService = applicantService;
        _vacancyService = vacancyService;
    }

    /// <summary>Список собеседований.</summary>
    public async Task<IActionResult> Index(InterviewFilterParams filter, string? reset, CancellationToken ct)
    {
        filter = ResolveFilter(filter, reset);
        await PopulateLookupsAsync(filter, ct);
        var model = await _service.GetPagedAsync(filter, ct);
        ViewBag.Filter = filter;
        return View(model);
    }

    /// <summary>Карточка собеседования.</summary>
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
        await PopulateLookupsAsync(null, ct);
        return View(new Interview { Date = DateOnly.FromDateTime(DateTime.Today) });
    }

    /// <summary>Создание собеседования.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind(nameof(Interview.Date), nameof(Interview.ApplicantId), nameof(Interview.VacancyId), nameof(Interview.Result))]
        Interview interview,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLookupsAsync(null, ct, interview.ApplicantId, interview.VacancyId);
            return View(interview);
        }

        try
        {
            await _service.CreateAsync(interview, ct);
            return RedirectToReturnOr(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await PopulateLookupsAsync(null, ct, interview.ApplicantId, interview.VacancyId);
            return View(interview);
        }
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

        await PopulateLookupsAsync(null, ct, entity.ApplicantId, entity.VacancyId);
        return View(entity);
    }

    /// <summary>Обновление собеседования.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind(nameof(Interview.Id), nameof(Interview.Date), nameof(Interview.ApplicantId), nameof(Interview.VacancyId), nameof(Interview.Result))]
        Interview interview,
        CancellationToken ct)
    {
        if (id != interview.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await PopulateLookupsAsync(null, ct, interview.ApplicantId, interview.VacancyId);
            return View(interview);
        }

        try
        {
            await _service.UpdateAsync(interview, ct);
            return RedirectToReturnOr(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await PopulateLookupsAsync(null, ct, interview.ApplicantId, interview.VacancyId);
            return View(interview);
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

    /// <summary>Удаление собеседования.</summary>
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

    private async Task PopulateLookupsAsync(
        InterviewFilterParams? filter,
        CancellationToken ct,
        int? selectedApplicantId = null,
        int? selectedVacancyId = null)
    {
        var applicants = await _applicantService.GetAllAsync(ct);
        var vacancies = await _vacancyService.GetAllAsync(ct);

        if (filter is not null)
        {
            ViewBag.ApplicantId = new SelectList(applicants, nameof(Applicant.Id), nameof(Applicant.FullName), filter.ApplicantId);
            ViewBag.VacancyId = new SelectList(vacancies, nameof(Vacancy.Id), nameof(Vacancy.Title), filter.VacancyId);
            ViewBag.Result = new SelectList(
                Enum.GetValues<InterviewResult>().Select(r => new { Value = (int)r, Text = r.ToString() }),
                "Value",
                "Text",
                filter.Result.HasValue ? (int)filter.Result.Value : null);
        }
        else
        {
            ViewBag.ApplicantId = new SelectList(applicants, nameof(Applicant.Id), nameof(Applicant.FullName), selectedApplicantId);
            ViewBag.VacancyId = new SelectList(vacancies, nameof(Vacancy.Id), nameof(Vacancy.Title), selectedVacancyId);
        }
    }
}
