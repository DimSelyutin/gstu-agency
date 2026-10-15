using Kursach.Web.Models.Entities;
using Kursach.Web.ViewModels;

namespace Kursach.Web.Services;

/// <summary>
/// Сервис справочника навыков.
/// </summary>
public interface ISkillService
{
    /// <summary>Возвращает постраничный список навыков.</summary>
    Task<PagedListViewModel<Skill>> GetPagedAsync(FilterParams filter, CancellationToken cancellationToken = default);

    /// <summary>Возвращает навык по идентификатору.</summary>
    Task<Skill?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Возвращает все навыки.</summary>
    Task<IReadOnlyList<Skill>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Создаёт навык.</summary>
    Task<int> CreateAsync(Skill entity, CancellationToken cancellationToken = default);

    /// <summary>Обновляет навык.</summary>
    Task UpdateAsync(Skill entity, CancellationToken cancellationToken = default);

    /// <summary>Удаляет навык.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
