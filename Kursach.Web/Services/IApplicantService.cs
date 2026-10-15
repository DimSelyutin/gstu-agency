using Kursach.Web.Models.Entities;
using Kursach.Web.ViewModels;

namespace Kursach.Web.Services;

/// <summary>
/// Сервис соискателей.
/// </summary>
public interface IApplicantService
{
    /// <summary>Возвращает постраничный список соискателей.</summary>
    Task<PagedListViewModel<Applicant>> GetPagedAsync(ApplicantFilterParams filter, CancellationToken cancellationToken = default);

    /// <summary>Возвращает соискателя по идентификатору.</summary>
    Task<Applicant?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Возвращает всех соискателей.</summary>
    Task<IReadOnlyList<Applicant>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Создаёт соискателя.</summary>
    Task<int> CreateAsync(ApplicantEditViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Обновляет соискателя.</summary>
    Task UpdateAsync(ApplicantEditViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Удаляет соискателя.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
