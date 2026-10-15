using FluentAssertions;
using Kursach.Web.Models.Entities;
using Kursach.Web.Services;
using Kursach.Web.ViewModels;

namespace Kursach.Tests.Services;

/// <summary>
/// Тесты сервиса вакансий и отчётов.
/// </summary>
public class VacancyServiceTests
{
    [Fact]
    public async Task MatchForApplicantAsync_RanksBySkillMatch_AndExcludesClosed()
    {
        await using var db = TestDbFactory.CreateContext();
        var industry = new Industry { Name = "IT" };
        db.Industries.Add(industry);
        await db.SaveChangesAsync();

        var company = new Company
        {
            Name = "Tech",
            IndustryId = industry.Id,
            Phone = "+375291111111",
            Email = "a@b.c"
        };
        db.Companies.Add(company);
        var skillA = new Skill { Name = "C#" };
        var skillB = new Skill { Name = "SQL" };
        db.Skills.AddRange(skillA, skillB);
        await db.SaveChangesAsync();

        var applicant = new Applicant
        {
            FullName = "Иванов",
            Education = EducationLevel.Higher,
            ExperienceYears = 3,
            DesiredSalary = 2000
        };
        db.Applicants.Add(applicant);
        await db.SaveChangesAsync();
        db.ApplicantSkills.AddRange(
            new ApplicantSkill { ApplicantId = applicant.Id, SkillId = skillA.Id, Level = 4 },
            new ApplicantSkill { ApplicantId = applicant.Id, SkillId = skillB.Id, Level = 2 });
        await db.SaveChangesAsync();

        var openFull = new Vacancy { Title = "Full", CompanyId = company.Id, Salary = 3000, Status = VacancyStatus.Open };
        var openPartial = new Vacancy { Title = "Partial", CompanyId = company.Id, Salary = 4000, Status = VacancyStatus.Open };
        var closed = new Vacancy { Title = "Closed", CompanyId = company.Id, Salary = 5000, Status = VacancyStatus.Closed, ClosedDate = DateOnly.FromDateTime(DateTime.Today) };
        db.Vacancies.AddRange(openFull, openPartial, closed);
        await db.SaveChangesAsync();

        db.VacancySkills.AddRange(
            new VacancySkill { VacancyId = openFull.Id, SkillId = skillA.Id, MinLevel = 3 },
            new VacancySkill { VacancyId = openFull.Id, SkillId = skillB.Id, MinLevel = 2 },
            new VacancySkill { VacancyId = openPartial.Id, SkillId = skillA.Id, MinLevel = 3 },
            new VacancySkill { VacancyId = openPartial.Id, SkillId = skillB.Id, MinLevel = 5 },
            new VacancySkill { VacancyId = closed.Id, SkillId = skillA.Id, MinLevel = 1 });
        await db.SaveChangesAsync();

        var service = new VacancyService(db);
        var matches = await service.MatchForApplicantAsync(applicant.Id);

        matches.Should().NotContain(m => m.Title == "Closed");
        matches.First().Title.Should().Be("Full");
        matches.First().MatchRatio.Should().Be(1);
        matches.Single(m => m.Title == "Partial").MatchRatio.Should().Be(0.5);
    }

    [Fact]
    public async Task GetOpenByIndustryAsync_FiltersOpenAndIndustry()
    {
        await using var db = TestDbFactory.CreateContext();
        var it = new Industry { Name = "IT" };
        var finance = new Industry { Name = "Finance" };
        db.Industries.AddRange(it, finance);
        await db.SaveChangesAsync();

        var c1 = new Company { Name = "C1", IndustryId = it.Id, Phone = "+375291111111", Email = "a@b.c" };
        var c2 = new Company { Name = "C2", IndustryId = finance.Id, Phone = "+375292222222", Email = "b@b.c" };
        db.Companies.AddRange(c1, c2);
        await db.SaveChangesAsync();

        db.Vacancies.AddRange(
            new Vacancy { Title = "Dev", CompanyId = c1.Id, Salary = 1000, Status = VacancyStatus.Open },
            new Vacancy { Title = "ClosedDev", CompanyId = c1.Id, Salary = 1000, Status = VacancyStatus.Closed, ClosedDate = DateOnly.FromDateTime(DateTime.Today) },
            new Vacancy { Title = "Banker", CompanyId = c2.Id, Salary = 1000, Status = VacancyStatus.Open });
        await db.SaveChangesAsync();

        var service = new VacancyService(db);
        var page = await service.GetOpenByIndustryAsync(it.Id, new FilterParams { PageSize = 20 });

        page.TotalCount.Should().Be(1);
        page.Items.Single().Title.Should().Be("Dev");
    }

    [Fact]
    public async Task GetClosedReportAsync_IncludesHiredApplicantAndTotals()
    {
        await using var db = TestDbFactory.CreateContext();
        var industry = new Industry { Name = "IT" };
        db.Industries.Add(industry);
        await db.SaveChangesAsync();
        var company = new Company { Name = "Co", IndustryId = industry.Id, Phone = "+375291111111", Email = "a@b.c" };
        db.Companies.Add(company);
        var applicant = new Applicant { FullName = "Петров", Education = EducationLevel.Higher, ExperienceYears = 1, DesiredSalary = 1500 };
        db.Applicants.Add(applicant);
        await db.SaveChangesAsync();

        var closedDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-10);
        var vacancy = new Vacancy
        {
            Title = "QA",
            CompanyId = company.Id,
            Salary = 2500,
            Status = VacancyStatus.Closed,
            ClosedDate = closedDate
        };
        db.Vacancies.Add(vacancy);
        await db.SaveChangesAsync();
        db.Interviews.Add(new Interview
        {
            Date = closedDate,
            ApplicantId = applicant.Id,
            VacancyId = vacancy.Id,
            Result = InterviewResult.Hired
        });
        await db.SaveChangesAsync();

        var service = new VacancyService(db);
        var report = await service.GetClosedReportAsync(closedDate.AddDays(-1), closedDate.AddDays(1));

        report.TotalCount.Should().Be(1);
        report.Items.Single().HiredApplicantName.Should().Be("Петров");
        report.AverageSalary.Should().Be(2500);
        report.TotalSalary.Should().Be(2500);
    }

    [Fact]
    public async Task GetPagedAsync_FiltersBySalaryAndStatus()
    {
        await using var db = TestDbFactory.CreateContext();
        var industry = new Industry { Name = "IT" };
        db.Industries.Add(industry);
        await db.SaveChangesAsync();
        var company = new Company { Name = "Co", IndustryId = industry.Id, Phone = "+375291111111", Email = "a@b.c" };
        db.Companies.Add(company);
        await db.SaveChangesAsync();
        db.Vacancies.AddRange(
            new Vacancy { Title = "Low", CompanyId = company.Id, Salary = 1000, Status = VacancyStatus.Open },
            new Vacancy { Title = "High", CompanyId = company.Id, Salary = 5000, Status = VacancyStatus.Open },
            new Vacancy { Title = "ClosedHigh", CompanyId = company.Id, Salary = 5000, Status = VacancyStatus.Closed, ClosedDate = DateOnly.FromDateTime(DateTime.Today) });
        await db.SaveChangesAsync();

        var service = new VacancyService(db);
        var page = await service.GetPagedAsync(new VacancyFilterParams
        {
            SalaryFrom = 2000,
            Status = VacancyStatus.Open,
            PageSize = 20
        });

        page.TotalCount.Should().Be(1);
        page.Items.Single().Title.Should().Be("High");
    }
}
