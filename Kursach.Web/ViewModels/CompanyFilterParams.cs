namespace Kursach.Web.ViewModels;

/// <summary>
/// Параметры фильтрации списка компаний.
/// </summary>
public class CompanyFilterParams : FilterParams
{
    /// <summary>Фильтр по сфере деятельности.</summary>
    public int? IndustryId { get; set; }
}
