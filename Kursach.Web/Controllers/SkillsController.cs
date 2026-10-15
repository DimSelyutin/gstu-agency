using Kursach.Web.Models.Entities;
using Kursach.Web.Services;
using Kursach.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kursach.Web.Controllers;

/// <summary>
/// CRUD для справочника навыков.
/// </summary>
public class SkillsController : AgencyControllerBase
{
    private readonly ISkillService _service;

    /// <summary>
    /// Создаёт экземпляр контроллера.
    /// </summary>
    public SkillsController(ISkillService service)
    {
        _service = service;
    }

    /// <summary>Список навыков.</summary>
    public async Task<IActionResult> Index(FilterParams filter, string? reset, CancellationToken ct)
    {
        filter = ResolveFilter(filter, reset);
        var model = await _service.GetPagedAsync(filter, ct);
        ViewBag.Filter = filter;
        return View(model);
    }

    /// <summary>Карточка навыка.</summary>
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
        return View(new Skill());
    }

    /// <summary>Создание навыка.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(nameof(Skill.Name))] Skill skill, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(skill);
        }

        try
        {
            await _service.CreateAsync(skill, ct);
            return RedirectToReturnOr(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(Skill.Name), ex.Message);
            return View(skill);
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

    /// <summary>Обновление навыка.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind(nameof(Skill.Id), nameof(Skill.Name))] Skill skill, CancellationToken ct)
    {
        if (id != skill.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(skill);
        }

        try
        {
            await _service.UpdateAsync(skill, ct);
            return RedirectToReturnOr(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(Skill.Name), ex.Message);
            return View(skill);
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

    /// <summary>Удаление навыка.</summary>
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
