namespace Kursach.Web.Data.Seed;

/// <summary>
/// Параметры объёма демонстрационных данных при сидинге.
/// </summary>
public class SeedOptions
{
    /// <summary>Количество сфер деятельности.</summary>
    public int IndustryCount { get; set; } = 100;

    /// <summary>Количество навыков.</summary>
    public int SkillCount { get; set; } = 100;

    /// <summary>Количество компаний.</summary>
    public int CompanyCount { get; set; } = 100;

    /// <summary>Количество соискателей.</summary>
    public int ApplicantCount { get; set; } = 100;

    /// <summary>Количество вакансий.</summary>
    public int VacancyCount { get; set; } = 100;

    /// <summary>Количество собеседований.</summary>
    public int InterviewCount { get; set; } = 10000;

    /// <summary>Фиксированный seed генератора случайных чисел.</summary>
    public int RandomSeed { get; set; } = 20261015;
}
