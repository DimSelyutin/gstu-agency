using FluentAssertions;
using Kursach.Web.Models.Entities;
using Kursach.Web.Services;
using Kursach.Web.ViewModels;

namespace Kursach.Tests.Services;

/// <summary>
/// Тесты сервиса собеседований.
/// </summary>
public class InterviewServiceTests
{
    [Fact]
    public async Task GetPagedAsync_FiltersByResultAndDate()
    {
        await using var db = TestDbFactory.CreateContext();
        var industry = new Industry { Name = "IT" };
        db.Industries.Add(industry);
        await db.SaveChangesAsync();
        var company = new Company { Name = "Co", IndustryId = industry.Id, Phone = "+375291111111", Email = "a@b.c" };
        db.Companies.Add(company);
        var applicant = new Applicant { FullName = "X", Education = EducationLevel.Higher, ExperienceYears = 1, DesiredSalary = 1000 };
        db.Applicants.Add(applicant);
        await db.SaveChangesAsync();
        var vacancy = new Vacancy { Title = "Dev", CompanyId = company.Id, Salary = 2000, Status = VacancyStatus.Open };
        db.Vacancies.Add(vacancy);
        await db.SaveChangesAsync();

        var today = DateOnly.FromDateTime(DateTime.Today);
        db.Interviews.AddRange(
            new Interview { Date = today, ApplicantId = applicant.Id, VacancyId = vacancy.Id, Result = InterviewResult.Hired },
            new Interview { Date = today.AddDays(-40), ApplicantId = applicant.Id, VacancyId = vacancy.Id, Result = InterviewResult.Failed },
            new Interview { Date = today.AddDays(-5), ApplicantId = applicant.Id, VacancyId = vacancy.Id, Result = InterviewResult.Passed });
        await db.SaveChangesAsync();

        var service = new InterviewService(db);
        var page = await service.GetPagedAsync(new InterviewFilterParams
        {
            DateFrom = today.AddDays(-10),
            Result = InterviewResult.Passed,
            PageSize = 20
        });

        page.TotalCount.Should().Be(1);
        page.Items.Single().Result.Should().Be(InterviewResult.Passed);
    }

    [Fact]
    public async Task CreateAsync_RejectsClosedVacancy()
    {
        await using var db = TestDbFactory.CreateContext();
        var industry = new Industry { Name = "IT" };
        db.Industries.Add(industry);
        await db.SaveChangesAsync();
        var company = new Company { Name = "Co", IndustryId = industry.Id, Phone = "+375291111111", Email = "a@b.c" };
        db.Companies.Add(company);
        var applicant = new Applicant { FullName = "X", Education = EducationLevel.Higher, ExperienceYears = 1, DesiredSalary = 1000 };
        db.Applicants.Add(applicant);
        await db.SaveChangesAsync();
        var vacancy = new Vacancy
        {
            Title = "Dev",
            CompanyId = company.Id,
            Salary = 2000,
            Status = VacancyStatus.Closed,
            ClosedDate = DateOnly.FromDateTime(DateTime.Today)
        };
        db.Vacancies.Add(vacancy);
        await db.SaveChangesAsync();

        var service = new InterviewService(db);
        var act = async () => await service.CreateAsync(new Interview
        {
            Date = DateOnly.FromDateTime(DateTime.Today),
            ApplicantId = applicant.Id,
            VacancyId = vacancy.Id,
            Result = InterviewResult.Scheduled
        });

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
