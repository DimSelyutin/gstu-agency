namespace Kursach.Web.ViewModels;

/// <summary>
/// Отчёт по закрытым вакансиям за период.
/// </summary>
public class ClosedVacancyReportViewModel
{
    /// <summary>Строки отчёта.</summary>
    public IReadOnlyList<ClosedVacancyReportItem> Items { get; set; } = Array.Empty<ClosedVacancyReportItem>();

    /// <summary>Начало периода (включительно).</summary>
    public DateOnly? From { get; set; }

    /// <summary>Конец периода (включительно).</summary>
    public DateOnly? To { get; set; }

    /// <summary>Количество закрытых вакансий.</summary>
    public int TotalCount { get; set; }

    /// <summary>Сумма окладов.</summary>
    public decimal TotalSalary { get; set; }

    /// <summary>Средний оклад.</summary>
    public decimal AverageSalary { get; set; }
}
