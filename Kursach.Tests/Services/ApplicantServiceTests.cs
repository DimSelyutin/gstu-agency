using FluentAssertions;
using Kursach.Web.Models.Entities;
using Kursach.Web.Services;
using Kursach.Web.ViewModels;

namespace Kursach.Tests.Services;

/// <summary>
/// Тесты сервиса соискателей.
/// </summary>
public class ApplicantServiceTests
{
    [Fact]
    public async Task GetPagedAsync_FiltersByEducationAndSalary()
    {
        await using var db = TestDbFactory.CreateContext();
        db.Applicants.AddRange(
            new Applicant { FullName = "А", Education = EducationLevel.Higher, ExperienceYears = 2, DesiredSalary = 1000 },
            new Applicant { FullName = "Б", Education = EducationLevel.Secondary, ExperienceYears = 1, DesiredSalary = 3000 },
            new Applicant { FullName = "В", Education = EducationLevel.Higher, ExperienceYears = 5, DesiredSalary = 2500 });
        await db.SaveChangesAsync();

        var service = new ApplicantService(db);
        var page = await service.GetPagedAsync(new ApplicantFilterParams
        {
            Education = EducationLevel.Higher,
            SalaryFrom = 2000,
            PageSize = 20
        });

        page.TotalCount.Should().Be(1);
        page.Items.Single().FullName.Should().Be("В");
    }

    [Fact]
    public async Task CreateAsync_SavesApplicantWithSkills()
    {
        await using var db = TestDbFactory.CreateContext();
        db.Skills.AddRange(new Skill { Name = "C#" }, new Skill { Name = "SQL" });
        await db.SaveChangesAsync();

        var service = new ApplicantService(db);
        var id = await service.CreateAsync(new ApplicantEditViewModel
        {
            FullName = "Сидоров",
            Education = EducationLevel.Higher,
            ExperienceYears = 4,
            DesiredSalary = 2200,
            Skills = db.Skills.Select(s => new ApplicantSkillItem { SkillId = s.Id, Level = 3 }).ToList()
        });

        var loaded = await service.GetByIdAsync(id);
        loaded.Should().NotBeNull();
        loaded!.ApplicantSkills.Should().HaveCount(2);
    }
}
