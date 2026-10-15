using Kursach.Web.Models.Entities;
using Kursach.Web.ViewModels;

namespace Kursach.Web.Services;

/// <summary>
/// Сервис компаний-работодателей.
/// </summary>
public interface ICompanyService
{
    /// <summary>Возвращает постраничный список компаний.</summary>
    Task<PagedListViewModel<Company>> GetPagedAsync(CompanyFilterParams filter, CancellationToken cancellationToken = default);

    /// <summary>Возвращает компанию по идентификатору.</summary>
    Task<Company?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Возвращает все компании.</summary>
    Task<IReadOnlyList<Company>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Создаёт компанию.</summary>
    Task<int> CreateAsync(Company entity, CancellationToken cancellationToken = default);

    /// <summary>Обновляет компанию.</summary>
    Task UpdateAsync(Company entity, CancellationToken cancellationToken = default);

    /// <summary>Удаляет компанию.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
