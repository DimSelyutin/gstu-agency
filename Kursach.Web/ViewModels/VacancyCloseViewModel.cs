using System.ComponentModel.DataAnnotations;

namespace Kursach.Web.ViewModels;

/// <summary>
/// Модель закрытия вакансии с выбором принятого кандидата.
/// </summary>
public class VacancyCloseViewModel
{
    /// <summary>Идентификатор вакансии.</summary>
    public int VacancyId { get; set; }

    /// <summary>Должность.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Идентификатор собеседования с принятым кандидатом.</summary>
    [Required(ErrorMessage = "Выберите кандидата")]
    [Display(Name = "Принятый кандидат")]
    public int HiredInterviewId { get; set; }
}
