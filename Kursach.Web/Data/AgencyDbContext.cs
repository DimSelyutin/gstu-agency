using Kursach.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kursach.Web.Data;

/// <summary>
/// Контекст Entity Framework Core для предметной области «Кадровое агентство».
/// </summary>
public class AgencyDbContext : DbContext
{
    /// <summary>
    /// Создаёт экземпляр контекста с заданными параметрами.
    /// </summary>
    /// <param name="options">Параметры конфигурации контекста.</param>
    public AgencyDbContext(DbContextOptions<AgencyDbContext> options)
        : base(options)
    {
    }

    /// <summary>Сферы деятельности.</summary>
    public DbSet<Industry> Industries => Set<Industry>();

    /// <summary>Навыки.</summary>
    public DbSet<Skill> Skills => Set<Skill>();

    /// <summary>Компании-работодатели.</summary>
    public DbSet<Company> Companies => Set<Company>();

    /// <summary>Соискатели.</summary>
    public DbSet<Applicant> Applicants => Set<Applicant>();

    /// <summary>Навыки соискателей.</summary>
    public DbSet<ApplicantSkill> ApplicantSkills => Set<ApplicantSkill>();

    /// <summary>Вакансии.</summary>
    public DbSet<Vacancy> Vacancies => Set<Vacancy>();

    /// <summary>Требования вакансий к навыкам.</summary>
    public DbSet<VacancySkill> VacancySkills => Set<VacancySkill>();

    /// <summary>Собеседования.</summary>
    public DbSet<Interview> Interviews => Set<Interview>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgencyDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
