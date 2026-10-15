using System.ComponentModel.DataAnnotations;

namespace Kursach.Web.Models.Entities;

/// <summary>
/// Компания-работодатель.
/// </summary>
public class Company
{
    /// <summary>Уникальный идентификатор.</summary>
    public int Id { get; set; }

    /// <summary>Название компании.</summary>
    [Required(ErrorMessage = "Укажите название компании")]
    [StringLength(200, ErrorMessage = "Название не должно превышать 200 символов")]
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Идентификатор сферы деятельности.</summary>
    [Required(ErrorMessage = "Выберите сферу деятельности")]
    [Display(Name = "Сфера деятельности")]
    public int IndustryId { get; set; }

    /// <summary>Сфера деятельности.</summary>
    public Industry? Industry { get; set; }

    /// <summary>Телефон.</summary>
    [Required(ErrorMessage = "Укажите телефон")]
    [Phone(ErrorMessage = "Некорректный номер телефона")]
    [StringLength(30, ErrorMessage = "Телефон не должен превышать 30 символов")]
    [Display(Name = "Телефон")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>Электронная почта.</summary>
    [Required(ErrorMessage = "Укажите e-mail")]
    [EmailAddress(ErrorMessage = "Некорректный e-mail")]
    [StringLength(150, ErrorMessage = "E-mail не должен превышать 150 символов")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Адрес (необязательно).</summary>
    [StringLength(250, ErrorMessage = "Адрес не должен превышать 250 символов")]
    [Display(Name = "Адрес")]
    public string? Address { get; set; }

    /// <summary>Вакансии компании.</summary>
    public ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
}
