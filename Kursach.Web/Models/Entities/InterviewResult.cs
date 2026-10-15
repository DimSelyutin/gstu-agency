using System.ComponentModel.DataAnnotations;

namespace Kursach.Web.Models.Entities;

/// <summary>
/// Результат собеседования.
/// </summary>
public enum InterviewResult
{
    /// <summary>Назначено.</summary>
    [Display(Name = "Назначено")]
    Scheduled = 0,

    /// <summary>Не пройдено.</summary>
    [Display(Name = "Не пройдено")]
    Failed = 1,

    /// <summary>Пройдено.</summary>
    [Display(Name = "Пройдено")]
    Passed = 2,

    /// <summary>Кандидат принят.</summary>
    [Display(Name = "Принят")]
    Hired = 3
}
