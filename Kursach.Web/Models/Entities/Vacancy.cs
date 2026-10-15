using System.ComponentModel.DataAnnotations;

namespace Kursach.Web.Models.Entities;

/// <summary>
/// Вакансия.
/// </summary>
public class Vacancy
{
    /// <summary>Уникальный идентификатор.</summary>
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

    /// <summary>Компания.</summary>
    public Company? Company { get; set; }

    /// <summary>Оклад.</summary>
    [Required(ErrorMessage = "Укажите оклад")]
    [Range(1, 1_000_000, ErrorMessage = "Оклад должен быть положительным числом")]
    [Display(Name = "Оклад")]
    [DataType(DataType.Currency)]
    public decimal Salary { get; set; }

    /// <summary>Описание / требования (текст).</summary>
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
    public ICollection<VacancySkill> VacancySkills { get; set; } = new List<VacancySkill>();

    /// <summary>Собеседования по вакансии.</summary>
    public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
}
