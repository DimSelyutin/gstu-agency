using Kursach.Web.Models.Entities;

namespace Kursach.Web.ViewModels;

/// <summary>
/// Параметры фильтрации списка вакансий.
/// </summary>
public class VacancyFilterParams : FilterParams
{
    /// <summary>Фильтр по сфере деятельности (через компанию).</summary>
    public int? IndustryId { get; set; }

    /// <summary>Фильтр по компании.</summary>
    public int? CompanyId { get; set; }

    /// <summary>Минимальный оклад.</summary>
    public decimal? SalaryFrom { get; set; }

    /// <summary>Максимальный оклад.</summary>
    public decimal? SalaryTo { get; set; }

    /// <summary>Фильтр по статусу вакансии.</summary>
    public VacancyStatus? Status { get; set; }
}
