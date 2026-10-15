using System.ComponentModel.DataAnnotations;

namespace Kursach.Web.Models.Entities;

/// <summary>
/// Сфера деятельности компаний.
/// </summary>
public class Industry
{
    /// <summary>Уникальный идентификатор.</summary>
    public int Id { get; set; }

    /// <summary>Название сферы.</summary>
    [Required(ErrorMessage = "Укажите название сферы")]
    [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Компании в данной сфере.</summary>
    public ICollection<Company> Companies { get; set; } = new List<Company>();
}
