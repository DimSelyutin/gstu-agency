using Kursach.Web.Models.Entities;

namespace Kursach.Web.ViewModels;

/// <summary>
/// Параметры фильтрации списка соискателей.
/// </summary>
public class ApplicantFilterParams : FilterParams
{
    /// <summary>Фильтр по уровню образования.</summary>
    public EducationLevel? Education { get; set; }

    /// <summary>Минимальная желаемая зарплата.</summary>
    public decimal? SalaryFrom { get; set; }

    /// <summary>Максимальная желаемая зарплата.</summary>
    public decimal? SalaryTo { get; set; }
}
