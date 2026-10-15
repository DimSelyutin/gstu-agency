namespace Kursach.Web.ViewModels;

/// <summary>
/// Базовые параметры фильтрации и пагинации списков.
/// </summary>
public class FilterParams
{
    /// <summary>Номер страницы (начиная с 1).</summary>
    public int Page { get; set; } = 1;

    /// <summary>Размер страницы.</summary>
    public int PageSize { get; set; } = 20;

    /// <summary>Поле сортировки.</summary>
    public string? SortBy { get; set; }

    /// <summary>Направление сортировки: asc или desc.</summary>
    public string SortDir { get; set; } = "asc";

    /// <summary>Текстовый поиск.</summary>
    public string? Search { get; set; }

    /// <summary>Нормализует номер и размер страницы.</summary>
    public void Normalize()
    {
        if (Page < 1)
        {
            Page = 1;
        }

        if (PageSize < 1)
        {
            PageSize = 20;
        }

        if (PageSize > 100)
        {
            PageSize = 100;
        }

        SortDir = string.Equals(SortDir, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";
    }
}
