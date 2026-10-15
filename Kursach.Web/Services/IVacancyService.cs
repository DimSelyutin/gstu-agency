using Kursach.Web.Models.Entities;
using Kursach.Web.ViewModels;

namespace Kursach.Web.Services;

/// <summary>
/// Сервис вакансий.
/// </summary>
public interface IVacancyService
{
    /// <summary>Возвращает постраничный список вакансий.</summary>
    Task<PagedListViewModel<Vacancy>> GetPagedAsync(VacancyFilterParams filter, CancellationToken cancellationToken = default);

    /// <summary>Возвращает вакансию по идентификатору.</summary>
    Task<Vacancy?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Возвращает все вакансии.</summary>
    Task<IReadOnlyList<Vacancy>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Создаёт вакансию.</summary>
    Task<int> CreateAsync(VacancyEditViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Обновляет вакансию.</summary>
    Task UpdateAsync(VacancyEditViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Удаляет вакансию.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Подбирает открытые вакансии для соискателя по навыкам.</summary>
    Task<IReadOnlyList<VacancyMatchViewModel>> MatchForApplicantAsync(int applicantId, CancellationToken cancellationToken = default);

    /// <summary>Возвращает открытые вакансии с фильтром по сфере.</summary>
    Task<PagedListViewModel<Vacancy>> GetOpenByIndustryAsync(int? industryId, FilterParams filter, CancellationToken cancellationToken = default);

    /// <summary>Формирует отчёт по закрытым вакансиям за период.</summary>
    Task<ClosedVacancyReportViewModel> GetClosedReportAsync(DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default);

    /// <summary>Закрывает вакансию с указанием принятого кандидата.</summary>
    Task CloseAsync(int vacancyId, int hiredInterviewId, CancellationToken cancellationToken = default);
}
