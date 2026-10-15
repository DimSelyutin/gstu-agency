#pragma warning disable 1591
using Kursach.Web.Data;
using Kursach.Web.Data.Providers;
using Kursach.Web.Data.Seed;
using Kursach.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAgencyDatabase(builder.Configuration);

builder.Services.Configure<SeedOptions>(builder.Configuration.GetSection("Seed"));
builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
builder.Services.AddScoped<IIndustryService, IndustryService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IApplicantService, ApplicantService>();
builder.Services.AddScoped<IVacancyService, VacancyService>();
builder.Services.AddScoped<IInterviewService, InterviewService>();

var app = builder.Build();

await DatabaseInitializer.InitializeAsync(app);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

/// <summary>
/// Точка входа веб-приложения (частичный класс для интеграционных тестов).
/// </summary>
public partial class Program;
