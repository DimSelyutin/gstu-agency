using Kursach.Web.Models.Entities;
using Kursach.Web.ViewModels;

namespace Kursach.Web.Services;

/// <summary>
/// Сервис справочника сфер деятельности.
/// </summary>
public interface IIndustryService
{
    /// <summary>Возвращает постраничный список сфер.</summary>
    Task<PagedListViewModel<Industry>> GetPagedAsync(FilterParams filter, CancellationToken cancellationToken = default);

    /// <summary>Возвращает сферу по идентификатору.</summary>
    Task<Industry?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Возвращает все сферы.</summary>
    Task<IReadOnlyList<Industry>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Создаёт сферу.</summary>
    Task<int> CreateAsync(Industry entity, CancellationToken cancellationToken = default);

    /// <summary>Обновляет сферу.</summary>
    Task UpdateAsync(Industry entity, CancellationToken cancellationToken = default);

    /// <summary>Удаляет сферу.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
