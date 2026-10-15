using Kursach.Web.Models.Entities;

namespace Kursach.Web.ViewModels;

/// <summary>
/// Параметры фильтрации списка собеседований.
/// </summary>
public class InterviewFilterParams : FilterParams
{
    /// <summary>Дата с (включительно).</summary>
    public DateOnly? DateFrom { get; set; }

    /// <summary>Дата по (включительно).</summary>
    public DateOnly? DateTo { get; set; }

    /// <summary>Фильтр по соискателю.</summary>
    public int? ApplicantId { get; set; }

    /// <summary>Фильтр по вакансии.</summary>
    public int? VacancyId { get; set; }

    /// <summary>Фильтр по результату.</summary>
    public InterviewResult? Result { get; set; }
}
