using Kursach.Web.Models.Entities;
using Kursach.Web.Services;
using Kursach.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kursach.Web.Controllers;

/// <summary>
/// CRUD для справочника сфер деятельности.
/// </summary>
public class IndustriesController : AgencyControllerBase
{
    private readonly IIndustryService _service;

    /// <summary>
    /// Создаёт экземпляр контроллера.
    /// </summary>
    public IndustriesController(IIndustryService service)
    {
        _service = service;
    }

    /// <summary>Список сфер.</summary>
    public async Task<IActionResult> Index(FilterParams filter, string? reset, CancellationToken ct)
    {
        filter = ResolveFilter(filter, reset);
        var model = await _service.GetPagedAsync(filter, ct);
        ViewBag.Filter = filter;
        return View(model);
    }

    /// <summary>Карточка сферы.</summary>
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
    public IActionResult Create()
    {
        RememberReturnUrl();
        return View(new Industry());
    }

    /// <summary>Создание сферы.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(nameof(Industry.Name))] Industry industry, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(industry);
        }

        try
        {
            await _service.CreateAsync(industry, ct);
            return RedirectToReturnOr(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(Industry.Name), ex.Message);
            return View(industry);
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

        return View(entity);
    }

    /// <summary>Обновление сферы.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind(nameof(Industry.Id), nameof(Industry.Name))] Industry industry, CancellationToken ct)
    {
        if (id != industry.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(industry);
        }

        try
        {
            await _service.UpdateAsync(industry, ct);
            return RedirectToReturnOr(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(Industry.Name), ex.Message);
            return View(industry);
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

    /// <summary>Удаление сферы.</summary>
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
}
