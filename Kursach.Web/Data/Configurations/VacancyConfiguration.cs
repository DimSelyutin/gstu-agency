using Kursach.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kursach.Web.Data.Configurations;

/// <summary>
/// Конфигурация сущности <see cref="Vacancy"/>.
/// </summary>
public class VacancyConfiguration : IEntityTypeConfiguration<Vacancy>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Vacancy> builder)
    {
        builder.ToTable("Vacancies");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Salary).HasPrecision(18, 2);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CompanyId);
        builder.HasIndex(x => x.Salary);
        builder.HasIndex(x => x.ClosedDate);

        builder.HasOne(x => x.Company)
            .WithMany(x => x.Vacancies)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
