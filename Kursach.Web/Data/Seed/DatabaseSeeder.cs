using Bogus;
using Kursach.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Kursach.Web.Data.Seed;

/// <summary>
/// Заполняет базу данных демонстрационными данными через библиотеку Bogus.
/// </summary>
public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly SeedOptions _options;

    private static readonly string[] IndustryNames =
    [
        "IT и разработка", "Финансы и банки", "Медицина", "Образование", "Строительство",
        "Розничная торговля", "Логистика", "Телекоммуникации", "Маркетинг и реклама", "Юриспруденция",
        "Производство", "Энергетика", "Сельское хозяйство", "Туризм и гостиницы", "Госслужба",
        "Страхование", "Недвижимость", "Автомобильная отрасль", "Фармацевтика", "Медиа и СМИ",
        "Консалтинг", "HR и рекрутинг", "Кибербезопасность", "Игровой индустрия", "E-commerce",
        "Авиация", "Железнодорожный транспорт", "Пищевая промышленность", "Химическая промышленность", "Металлургия"
    ];

    private static readonly string[] SkillNames =
    [
        "C#", "Java", "Python", "JavaScript", "TypeScript", "SQL", "PostgreSQL", "MS SQL",
        "Entity Framework", "ASP.NET Core", "React", "Angular", "Vue.js", "Docker", "Kubernetes",
        "Git", "Linux", "Excel", "Power BI", "1C", "Бухгалтерский учёт", "Налоговое право",
        "Английский язык", "Немецкий язык", "Переговоры", "Управление проектами", "Scrum",
        "Тестирование ПО", "DevOps", "AWS", "Azure", "GCP", "Машинное обучение", "Анализ данных",
        "UI/UX дизайн", "Figma", "Photoshop", "Копирайтинг", "SMM", "SEO", "Контекстная реклама",
        "Хирургия", "Терапия", "Фармакология", "Педагогика", "Психология", "Юридическое консультирование",
        "Кадровое делопроизводство", "Охрана труда", "Логистика поставок", "Складской учёт", "Вождение категории B",
        "Вождение категории C", "Сварочные работы", "Электромонтаж", "Черчение AutoCAD", "SolidWorks",
        "CNC-станок", "QA Manual", "QA Automation", "REST API", "GraphQL", "Microservices", "Redis",
        "RabbitMQ", "Kafka", "MongoDB", "NoSQL", "CI/CD", "Terraform", "Ansible", "Bash",
        "PowerShell", "Swift", "Kotlin", "Flutter", "Go", "Rust", "PHP", "Laravel", "Spring Boot",
        "Hibernate", "JUnit", "xUnit", "NUnit", "Cypress", "Selenium", "Jira", "Confluence",
        "Customer Support", "Холодные звонки", "Продажи B2B", "Продажи B2C", "CRM Salesforce",
        "Bitrix24", "AmoCRM", "Финансовый анализ", "Аудит", "Risk Management", "Compliance"
    ];

    /// <summary>
    /// Создаёт сидер с параметрами объёма данных.
    /// </summary>
    public DatabaseSeeder(IOptions<SeedOptions>? options = null)
    {
        _options = options?.Value ?? new SeedOptions();
    }

    /// <inheritdoc />
    public async Task SeedAsync(AgencyDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Industries.AnyAsync(cancellationToken))
        {
            return;
        }

        Randomizer.Seed = new Random(_options.RandomSeed);
        var faker = new Faker("ru");

        var industries = Enumerable.Range(0, _options.IndustryCount)
            .Select(i => new Industry
            {
                Name = i < IndustryNames.Length
                    ? IndustryNames[i]
                    : $"{IndustryNames[i % IndustryNames.Length]} #{i + 1}"
            })
            .ToList();
        await db.Industries.AddRangeAsync(industries, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var skills = Enumerable.Range(0, _options.SkillCount)
            .Select(i => new Skill
            {
                Name = i < SkillNames.Length
                    ? SkillNames[i]
                    : $"{SkillNames[i % SkillNames.Length]} #{i + 1}"
            })
            .ToList();
        await db.Skills.AddRangeAsync(skills, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var industryIds = industries.Select(x => x.Id).ToList();
        var companies = new Faker<Company>("ru")
            .RuleFor(c => c.Name, f => f.Company.CompanyName())
            .RuleFor(c => c.IndustryId, f => f.PickRandom(industryIds))
            .RuleFor(c => c.Phone, f => $"+375{f.Random.Int(17, 44)}{f.Random.Int(1000000, 9999999)}")
            .RuleFor(c => c.Email, (f, c) => f.Internet.Email(c.Name.Replace(" ", "").ToLowerInvariant()))
            .RuleFor(c => c.Address, f => f.Address.StreetAddress())
            .Generate(_options.CompanyCount);
        await db.Companies.AddRangeAsync(companies, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var educationValues = Enum.GetValues<EducationLevel>();
        var applicants = new Faker<Applicant>("ru")
            .RuleFor(a => a.FullName, f => f.Name.FullName())
            .RuleFor(a => a.Education, f => f.PickRandom(educationValues))
            .RuleFor(a => a.ExperienceYears, f => f.Random.Int(0, 35))
            .RuleFor(a => a.DesiredSalary, f => f.Finance.Amount(800, 8000, 0))
            .Generate(_options.ApplicantCount);
        await db.Applicants.AddRangeAsync(applicants, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var skillIds = skills.Select(s => s.Id).ToList();
        var applicantSkills = new List<ApplicantSkill>();
        foreach (var applicant in applicants)
        {
            var count = faker.Random.Int(2, Math.Min(6, skillIds.Count));
            var selected = faker.Random.ListItems(skillIds, count).Distinct();
            foreach (var skillId in selected)
            {
                applicantSkills.Add(new ApplicantSkill
                {
                    ApplicantId = applicant.Id,
                    SkillId = skillId,
                    Level = faker.Random.Int(1, 5)
                });
            }
        }

        await db.ApplicantSkills.AddRangeAsync(applicantSkills, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var companyIds = companies.Select(c => c.Id).ToList();
        var vacancies = new Faker<Vacancy>("ru")
            .RuleFor(v => v.Title, f => f.Name.JobTitle())
            .RuleFor(v => v.CompanyId, f => f.PickRandom(companyIds))
            .RuleFor(v => v.Salary, f => f.Finance.Amount(1000, 10000, 0))
            .RuleFor(v => v.Description, f => f.Lorem.Sentence(12))
            .RuleFor(v => v.Status, f => f.Random.Bool(0.3f) ? VacancyStatus.Closed : VacancyStatus.Open)
            .RuleFor(v => v.ClosedDate, (f, v) =>
                v.Status == VacancyStatus.Closed
                    ? DateOnly.FromDateTime(DateTime.Today).AddDays(-f.Random.Int(1, 180))
                    : null)
            .Generate(_options.VacancyCount);
        await db.Vacancies.AddRangeAsync(vacancies, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var vacancySkills = new List<VacancySkill>();
        foreach (var vacancy in vacancies)
        {
            var count = faker.Random.Int(3, Math.Min(5, skillIds.Count));
            var selected = faker.Random.ListItems(skillIds, count).Distinct();
            foreach (var skillId in selected)
            {
                vacancySkills.Add(new VacancySkill
                {
                    VacancyId = vacancy.Id,
                    SkillId = skillId,
                    MinLevel = faker.Random.Int(1, 4)
                });
            }
        }

        await db.VacancySkills.AddRangeAsync(vacancySkills, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var applicantIds = applicants.Select(a => a.Id).ToList();
        var vacancyIds = vacancies.Select(v => v.Id).ToList();
        var closedVacancies = vacancies.Where(v => v.Status == VacancyStatus.Closed).ToList();
        var openVacancyIds = vacancies.Where(v => v.Status == VacancyStatus.Open).Select(v => v.Id).ToList();
        var resultValues = Enum.GetValues<InterviewResult>();
        var today = DateOnly.FromDateTime(DateTime.Today);

        var interviews = new List<Interview>(_options.InterviewCount);

        // Гарантируем собеседование «Принят» для каждой закрытой вакансии.
        foreach (var vacancy in closedVacancies)
        {
            interviews.Add(new Interview
            {
                Date = vacancy.ClosedDate ?? today.AddDays(-faker.Random.Int(1, 60)),
                ApplicantId = faker.PickRandom(applicantIds),
                VacancyId = vacancy.Id,
                Result = InterviewResult.Hired
            });
        }

        var remaining = Math.Max(0, _options.InterviewCount - interviews.Count);
        for (var i = 0; i < remaining; i++)
        {
            var vacancyId = openVacancyIds.Count > 0 && faker.Random.Bool(0.7f)
                ? faker.PickRandom(openVacancyIds)
                : faker.PickRandom(vacancyIds);
            var result = faker.PickRandom(resultValues.Where(r => r != InterviewResult.Hired).ToArray());
            if (closedVacancies.Any(v => v.Id == vacancyId) && faker.Random.Bool(0.2f))
            {
                result = InterviewResult.Hired;
            }

            interviews.Add(new Interview
            {
                Date = today.AddDays(faker.Random.Int(-365, 30)),
                ApplicantId = faker.PickRandom(applicantIds),
                VacancyId = vacancyId,
                Result = result
            });
        }

        const int batchSize = 1000;
        for (var i = 0; i < interviews.Count; i += batchSize)
        {
            var batch = interviews.Skip(i).Take(batchSize);
            await db.Interviews.AddRangeAsync(batch, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
