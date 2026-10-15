using System.ComponentModel.DataAnnotations;

namespace Kursach.Web.Models.Entities;

/// <summary>
/// Связь соискателя и навыка с уровнем владения.
/// </summary>
public class ApplicantSkill
{
    /// <summary>Идентификатор соискателя.</summary>
    [Display(Name = "Соискатель")]
    public int ApplicantId { get; set; }

    /// <summary>Соискатель.</summary>
    public Applicant? Applicant { get; set; }

    /// <summary>Идентификатор навыка.</summary>
    [Display(Name = "Навык")]
    public int SkillId { get; set; }

    /// <summary>Навык.</summary>
    public Skill? Skill { get; set; }

    /// <summary>Уровень владения (1–5).</summary>
    [Required(ErrorMessage = "Укажите уровень владения")]
    [Range(1, 5, ErrorMessage = "Уровень владения должен быть от 1 до 5")]
    [Display(Name = "Уровень")]
    public int Level { get; set; }
}
