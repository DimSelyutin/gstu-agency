using System.ComponentModel.DataAnnotations;

namespace Kursach.Web.Models.Entities;

/// <summary>
/// Уровень образования соискателя.
/// </summary>
public enum EducationLevel
{
    /// <summary>Среднее образование.</summary>
    [Display(Name = "Среднее")]
    Secondary = 0,

    /// <summary>Среднее специальное.</summary>
    [Display(Name = "Среднее специальное")]
    SecondarySpecial = 1,

    /// <summary>Неполное высшее.</summary>
    [Display(Name = "Неполное высшее")]
    IncompleteHigher = 2,

    /// <summary>Высшее.</summary>
    [Display(Name = "Высшее")]
    Higher = 3,

    /// <summary>Магистратура / учёная степень.</summary>
    [Display(Name = "Магистратура / учёная степень")]
    Postgraduate = 4
}
