namespace Kursach.Web.ViewModels;

/// <summary>
/// Строка отчёта по закрытым вакансиям.
/// </summary>
public class ClosedVacancyReportItem
{
    /// <summary>Идентификатор вакансии.</summary>
    public int VacancyId { get; set; }

    /// <summary>Должность.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Название компании.</summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>ФИО принятого соискателя.</summary>
    public string HiredApplicantName { get; set; } = string.Empty;

    /// <summary>Дата закрытия вакансии.</summary>
    public DateOnly ClosedDate { get; set; }

    /// <summary>Оклад.</summary>
    public decimal Salary { get; set; }
}
