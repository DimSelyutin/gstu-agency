using FluentAssertions;
using Kursach.Web.Data.Seed;
using Microsoft.Extensions.Options;

namespace Kursach.Tests.Data;

/// <summary>
/// Тесты идемпотентного сидинга БД.
/// </summary>
public class DatabaseSeederTests
{
    [Fact]
    public async Task SeedAsync_FillsEmptyDatabase()
    {
        await using var db = TestDbFactory.CreateContext();
        var seeder = new DatabaseSeeder(Options.Create(new SeedOptions
        {
            IndustryCount = 5,
            SkillCount = 5,
            CompanyCount = 5,
            ApplicantCount = 5,
            VacancyCount = 5,
            InterviewCount = 30,
            RandomSeed = 42
        }));

        await seeder.SeedAsync(db);

        db.Industries.Count().Should().Be(5);
        db.Skills.Count().Should().Be(5);
        db.Companies.Count().Should().Be(5);
        db.Applicants.Count().Should().Be(5);
        db.Vacancies.Count().Should().Be(5);
        db.Interviews.Count().Should().Be(30);
        db.ApplicantSkills.Any().Should().BeTrue();
        db.VacancySkills.Any().Should().BeTrue();
        db.Vacancies.Where(v => v.Status == Kursach.Web.Models.Entities.VacancyStatus.Closed)
            .All(v => db.Interviews.Any(i => i.VacancyId == v.Id && i.Result == Kursach.Web.Models.Entities.InterviewResult.Hired))
            .Should().BeTrue();
    }

    [Fact]
    public async Task SeedAsync_IsIdempotent()
    {
        await using var db = TestDbFactory.CreateContext();
        var seeder = new DatabaseSeeder(Options.Create(new SeedOptions
        {
            IndustryCount = 3,
            SkillCount = 3,
            CompanyCount = 3,
            ApplicantCount = 3,
            VacancyCount = 3,
            InterviewCount = 10,
            RandomSeed = 7
        }));

        await seeder.SeedAsync(db);
        await seeder.SeedAsync(db);

        db.Industries.Count().Should().Be(3);
        db.Interviews.Count().Should().Be(10);
    }
}
