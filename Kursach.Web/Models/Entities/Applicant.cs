using System.ComponentModel.DataAnnotations;

namespace Kursach.Web.Models.Entities;

/// <summary>
/// Соискатель.
/// </summary>
public class Applicant
{
    /// <summary>Уникальный идентификатор.</summary>
    public int Id { get; set; }

    /// <summary>ФИО.</summary>
    [Required(ErrorMessage = "Укажите ФИО")]
    [StringLength(150, ErrorMessage = "ФИО не должно превышать 150 символов")]
    [Display(Name = "ФИО")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>Образование.</summary>
    [Required(ErrorMessage = "Укажите образование")]
    [Display(Name = "Образование")]
    public EducationLevel Education { get; set; }

    /// <summary>Опыт работы (лет).</summary>
    [Required(ErrorMessage = "Укажите опыт работы")]
    [Range(0, 60, ErrorMessage = "Опыт работы должен быть от 0 до 60 лет")]
    [Display(Name = "Опыт работы (лет)")]
    public int ExperienceYears { get; set; }

    /// <summary>Желаемая зарплата.</summary>
    [Required(ErrorMessage = "Укажите желаемую зарплату")]
    [Range(1, 1_000_000, ErrorMessage = "Желаемая зарплата должна быть положительным числом")]
    [Display(Name = "Желаемая зарплата")]
    [DataType(DataType.Currency)]
    public decimal DesiredSalary { get; set; }

    /// <summary>Навыки соискателя.</summary>
    public ICollection<ApplicantSkill> ApplicantSkills { get; set; } = new List<ApplicantSkill>();

    /// <summary>Собеседования.</summary>
    public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
}
