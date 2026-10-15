using Kursach.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kursach.Web.Data.Configurations;

/// <summary>
/// Конфигурация сущности <see cref="Applicant"/>.
/// </summary>
public class ApplicantConfiguration : IEntityTypeConfiguration<Applicant>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Applicant> builder)
    {
        builder.ToTable("Applicants");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FullName).IsRequired().HasMaxLength(150);
        builder.Property(x => x.DesiredSalary).HasPrecision(18, 2);
        builder.HasIndex(x => x.FullName);
        builder.HasIndex(x => x.Education);
        builder.HasIndex(x => x.DesiredSalary);
    }
}
