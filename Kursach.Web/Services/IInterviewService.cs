using Kursach.Web.Models.Entities;
using Kursach.Web.ViewModels;

namespace Kursach.Web.Services;

/// <summary>
/// Сервис собеседований.
/// </summary>
public interface IInterviewService
{
    /// <summary>Возвращает постраничный список собеседований.</summary>
    Task<PagedListViewModel<Interview>> GetPagedAsync(InterviewFilterParams filter, CancellationToken cancellationToken = default);

    /// <summary>Возвращает собеседование по идентификатору.</summary>
    Task<Interview?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Создаёт собеседование.</summary>
    Task<int> CreateAsync(Interview entity, CancellationToken cancellationToken = default);

    /// <summary>Обновляет собеседование.</summary>
    Task UpdateAsync(Interview entity, CancellationToken cancellationToken = default);

    /// <summary>Удаляет собеседование.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
