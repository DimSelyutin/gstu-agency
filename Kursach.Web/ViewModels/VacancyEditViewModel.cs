using System.ComponentModel.DataAnnotations;
using Kursach.Web.Models.Entities;

namespace Kursach.Web.ViewModels;

/// <summary>
/// Требование к навыку в форме редактирования вакансии.
/// </summary>
public class VacancySkillItem
{
    /// <summary>Идентификатор навыка.</summary>
    [Display(Name = "Навык")]
    public int SkillId { get; set; }

    /// <summary>Минимальный требуемый уровень (1–5).</summary>
    [Range(1, 5, ErrorMessage = "Минимальный уровень должен быть от 1 до 5")]
    [Display(Name = "Мин. уровень")]
    public int MinLevel { get; set; }
}

/// <summary>
/// Модель создания и редактирования вакансии.
/// </summary>
public class VacancyEditViewModel
{
    /// <summary>Идентификатор (0 при создании).</summary>
    public int Id { get; set; }

    /// <summary>Должность.</summary>
    [Required(ErrorMessage = "Укажите должность")]
    [StringLength(150, ErrorMessage = "Должность не должна превышать 150 символов")]
    [Display(Name = "Должность")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Идентификатор компании.</summary>
    [Required(ErrorMessage = "Выберите компанию")]
    [Display(Name = "Компания")]
    public int CompanyId { get; set; }

    /// <summary>Оклад.</summary>
    [Required(ErrorMessage = "Укажите оклад")]
    [Range(1, 1_000_000, ErrorMessage = "Оклад должен быть положительным числом")]
    [Display(Name = "Оклад")]
    [DataType(DataType.Currency)]
    public decimal Salary { get; set; }

    /// <summary>Описание / требования.</summary>
    [StringLength(2000, ErrorMessage = "Описание не должно превышать 2000 символов")]
    [Display(Name = "Описание")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    /// <summary>Статус вакансии.</summary>
    [Required]
    [Display(Name = "Статус")]
    public VacancyStatus Status { get; set; } = VacancyStatus.Open;

    /// <summary>Дата закрытия (если закрыта).</summary>
    [DataType(DataType.Date)]
    [Display(Name = "Дата закрытия")]
    public DateOnly? ClosedDate { get; set; }

    /// <summary>Требуемые навыки.</summary>
    public List<VacancySkillItem> Skills { get; set; } = new();

    /// <summary>Доступные навыки для выбора (заполняется контроллером).</summary>
    public IReadOnlyList<Skill> AvailableSkills { get; set; } = Array.Empty<Skill>();
}
