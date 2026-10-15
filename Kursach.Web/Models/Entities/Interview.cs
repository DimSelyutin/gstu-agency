using System.ComponentModel.DataAnnotations;

namespace Kursach.Web.Models.Entities;

/// <summary>
/// Собеседование.
/// </summary>
public class Interview
{
    /// <summary>Уникальный идентификатор.</summary>
    public int Id { get; set; }

    /// <summary>Дата собеседования.</summary>
    [Required(ErrorMessage = "Укажите дату собеседования")]
    [DataType(DataType.Date)]
    [Display(Name = "Дата")]
    public DateOnly Date { get; set; }

    /// <summary>Идентификатор соискателя.</summary>
    [Required(ErrorMessage = "Выберите соискателя")]
    [Display(Name = "Соискатель")]
    public int ApplicantId { get; set; }

    /// <summary>Соискатель.</summary>
    public Applicant? Applicant { get; set; }

    /// <summary>Идентификатор вакансии.</summary>
    [Required(ErrorMessage = "Выберите вакансию")]
    [Display(Name = "Вакансия")]
    public int VacancyId { get; set; }

    /// <summary>Вакансия.</summary>
    public Vacancy? Vacancy { get; set; }

    /// <summary>Результат.</summary>
    [Required(ErrorMessage = "Укажите результат")]
    [Display(Name = "Результат")]
    public InterviewResult Result { get; set; } = InterviewResult.Scheduled;
}
