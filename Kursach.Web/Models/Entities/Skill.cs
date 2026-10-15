using System.ComponentModel.DataAnnotations;

namespace Kursach.Web.Models.Entities;

/// <summary>
/// Навык (справочник).
/// </summary>
public class Skill
{
    /// <summary>Уникальный идентификатор.</summary>
    public int Id { get; set; }

    /// <summary>Название навыка.</summary>
    [Required(ErrorMessage = "Укажите название навыка")]
    [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Связи с соискателями.</summary>
    public ICollection<ApplicantSkill> ApplicantSkills { get; set; } = new List<ApplicantSkill>();

    /// <summary>Связи с вакансиями.</summary>
    public ICollection<VacancySkill> VacancySkills { get; set; } = new List<VacancySkill>();
}
