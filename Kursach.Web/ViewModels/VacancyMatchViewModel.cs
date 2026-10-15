namespace Kursach.Web.ViewModels;

/// <summary>
/// Вакансия, подобранная для соискателя по навыкам.
/// </summary>
public class VacancyMatchViewModel
{
    /// <summary>Идентификатор вакансии.</summary>
    public int VacancyId { get; set; }

    /// <summary>Должность.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Название компании.</summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>Оклад.</summary>
    public decimal Salary { get; set; }

    /// <summary>Название сферы деятельности.</summary>
    public string IndustryName { get; set; } = string.Empty;

    /// <summary>Количество совпавших требований по навыкам.</summary>
    public int MatchedSkills { get; set; }

    /// <summary>Общее количество требований по навыкам.</summary>
    public int RequiredSkills { get; set; }

    /// <summary>Доля совпадения (0–1).</summary>
    public double MatchRatio { get; set; }
}
