namespace Kursach.Web.ViewModels;

/// <summary>
/// Универсальная модель постраничного списка.
/// </summary>
/// <typeparam name="T">Тип элементов списка.</typeparam>
public class PagedListViewModel<T>
{
    /// <summary>Элементы текущей страницы.</summary>
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();

    /// <summary>Номер текущей страницы (начиная с 1).</summary>
    public int PageNumber { get; init; }

    /// <summary>Размер страницы.</summary>
    public int PageSize { get; init; }

    /// <summary>Общее количество записей.</summary>
    public int TotalCount { get; init; }

    /// <summary>Общее количество страниц.</summary>
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>Есть ли предыдущая страница.</summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>Есть ли следующая страница.</summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>Поле сортировки.</summary>
    public string? SortBy { get; init; }

    /// <summary>Направление сортировки (asc/desc).</summary>
    public string? SortDir { get; init; }
}
