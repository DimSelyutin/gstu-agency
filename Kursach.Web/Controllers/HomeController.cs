using System.Diagnostics;
using Kursach.Web.Data;
using Kursach.Web.Models;
using Kursach.Web.Models.Entities;
using Kursach.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kursach.Web.Controllers;

/// <summary>
/// Главная страница и служебные действия.
/// </summary>
public class HomeController : Controller
{
    private readonly AgencyDbContext _db;

    /// <summary>
    /// Создаёт экземпляр контроллера.
    /// </summary>
    public HomeController(AgencyDbContext db)
    {
        _db = db;
    }

    /// <summary>Панель управления.</summary>
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var model = new HomeIndexViewModel
        {
            ApplicantCount = await _db.Applicants.CountAsync(ct),
            CompanyCount = await _db.Companies.CountAsync(ct),
            VacancyCount = await _db.Vacancies.CountAsync(ct),
            OpenVacancyCount = await _db.Vacancies.CountAsync(v => v.Status == VacancyStatus.Open, ct),
            InterviewCount = await _db.Interviews.CountAsync(ct)
        };

        return View(model);
    }

    /// <summary>Страница конфиденциальности (шаблон).</summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>Страница ошибки.</summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
