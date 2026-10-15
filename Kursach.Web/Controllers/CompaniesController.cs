using Kursach.Web.Models.Entities;
using Kursach.Web.Services;
using Kursach.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kursach.Web.Controllers;

/// <summary>
/// CRUD для компаний-работодателей.
/// </summary>
public class CompaniesController : AgencyControllerBase
{
    private readonly ICompanyService _service;
    private readonly IIndustryService _industryService;

    /// <summary>
    /// Создаёт экземпляр контроллера.
    /// </summary>
    public CompaniesController(ICompanyService service, IIndustryService industryService)
    {
        _service = service;
        _industryService = industryService;
    }

    /// <summary>Список компаний.</summary>
    public async Task<IActionResult> Index(CompanyFilterParams filter, string? reset, CancellationToken ct)
    {
        filter = ResolveFilter(filter, reset);
        await PopulateIndustrySelectListAsync(filter.IndustryId, ct);
        var model = await _service.GetPagedAsync(filter, ct);
        ViewBag.Filter = filter;
        return View(model);
    }

    /// <summary>Карточка компании.</summary>
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
        await PopulateIndustrySelectListAsync(null, ct);
        return View(new Company());
    }

    /// <summary>Создание компании.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind(nameof(Company.Name), nameof(Company.IndustryId), nameof(Company.Phone), nameof(Company.Email), nameof(Company.Address))]
        Company company,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateIndustrySelectListAsync(company.IndustryId, ct);
            return View(company);
        }

        await _service.CreateAsync(company, ct);
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

        await PopulateIndustrySelectListAsync(entity.IndustryId, ct);
        return View(entity);
    }

    /// <summary>Обновление компании.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind(nameof(Company.Id), nameof(Company.Name), nameof(Company.IndustryId), nameof(Company.Phone), nameof(Company.Email), nameof(Company.Address))]
        Company company,
        CancellationToken ct)
    {
        if (id != company.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await PopulateIndustrySelectListAsync(company.IndustryId, ct);
            return View(company);
        }

        await _service.UpdateAsync(company, ct);
        return RedirectToReturnOr(nameof(Index));
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

    /// <summary>Удаление компании.</summary>
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

    private async Task PopulateIndustrySelectListAsync(int? selectedId, CancellationToken ct)
    {
        var industries = await _industryService.GetAllAsync(ct);
        ViewBag.IndustryId = new SelectList(industries, nameof(Industry.Id), nameof(Industry.Name), selectedId);
    }
}
