namespace Kursach.Web.ViewModels;

/// <summary>
/// Сводка для главной страницы.
/// </summary>
public class HomeIndexViewModel
{
    /// <summary>Количество соискателей.</summary>
    public int ApplicantCount { get; set; }

    /// <summary>Количество компаний.</summary>
    public int CompanyCount { get; set; }

    /// <summary>Количество вакансий.</summary>
    public int VacancyCount { get; set; }

    /// <summary>Количество открытых вакансий.</summary>
    public int OpenVacancyCount { get; set; }

    /// <summary>Количество собеседований.</summary>
    public int InterviewCount { get; set; }
}
