using System.ComponentModel.DataAnnotations;
using Kursach.Web.Models.Entities;

namespace Kursach.Web.ViewModels;

/// <summary>
/// Навык соискателя в форме редактирования.
/// </summary>
public class ApplicantSkillItem
{
    /// <summary>Идентификатор навыка.</summary>
    [Display(Name = "Навык")]
    public int SkillId { get; set; }

    /// <summary>Уровень владения (1–5).</summary>
    [Range(1, 5, ErrorMessage = "Уровень владения должен быть от 1 до 5")]
    [Display(Name = "Уровень")]
    public int Level { get; set; }
}

/// <summary>
/// Модель создания и редактирования соискателя.
/// </summary>
public class ApplicantEditViewModel
{
    /// <summary>Идентификатор (0 при создании).</summary>
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

    /// <summary>Выбранные навыки и уровни.</summary>
    public List<ApplicantSkillItem> Skills { get; set; } = new();

    /// <summary>Доступные навыки для выбора (заполняется контроллером).</summary>
    public IReadOnlyList<Skill> AvailableSkills { get; set; } = Array.Empty<Skill>();
}
