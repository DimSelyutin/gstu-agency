using System.ComponentModel.DataAnnotations;

namespace Kursach.Web.Models.Entities;

/// <summary>
/// Статус вакансии.
/// </summary>
public enum VacancyStatus
{
    /// <summary>Открыта.</summary>
    [Display(Name = "Открыта")]
    Open = 0,

    /// <summary>Закрыта.</summary>
    [Display(Name = "Закрыта")]
    Closed = 1
}
