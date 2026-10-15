using System.ComponentModel.DataAnnotations;

namespace Kursach.Web.Models.Entities;

/// <summary>
/// Требование вакансии к навыку с минимальным уровнем.
/// </summary>
public class VacancySkill
{
    /// <summary>Идентификатор вакансии.</summary>
    [Display(Name = "Вакансия")]
    public int VacancyId { get; set; }

    /// <summary>Вакансия.</summary>
    public Vacancy? Vacancy { get; set; }

    /// <summary>Идентификатор навыка.</summary>
    [Display(Name = "Навык")]
    public int SkillId { get; set; }

    /// <summary>Навык.</summary>
    public Skill? Skill { get; set; }

    /// <summary>Минимальный требуемый уровень (1–5).</summary>
    [Required(ErrorMessage = "Укажите минимальный уровень")]
    [Range(1, 5, ErrorMessage = "Минимальный уровень должен быть от 1 до 5")]
    [Display(Name = "Мин. уровень")]
    public int MinLevel { get; set; }
}
