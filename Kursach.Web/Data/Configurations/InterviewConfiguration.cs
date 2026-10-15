using Kursach.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kursach.Web.Data.Configurations;

/// <summary>
/// Конфигурация сущности <see cref="Interview"/>.
/// </summary>
public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Interview> builder)
    {
        builder.ToTable("Interviews");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Date);
        builder.HasIndex(x => new { x.VacancyId, x.Result });
        builder.HasIndex(x => x.ApplicantId);

        builder.HasOne(x => x.Applicant)
            .WithMany(x => x.Interviews)
            .HasForeignKey(x => x.ApplicantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Vacancy)
            .WithMany(x => x.Interviews)
            .HasForeignKey(x => x.VacancyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
