using Kursach.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kursach.Web.Data.Configurations;

/// <summary>
/// Конфигурация сущности <see cref="ApplicantSkill"/>.
/// </summary>
public class ApplicantSkillConfiguration : IEntityTypeConfiguration<ApplicantSkill>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ApplicantSkill> builder)
    {
        builder.ToTable("ApplicantSkills");
        builder.HasKey(x => new { x.ApplicantId, x.SkillId });

        builder.HasOne(x => x.Applicant)
            .WithMany(x => x.ApplicantSkills)
            .HasForeignKey(x => x.ApplicantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Skill)
            .WithMany(x => x.ApplicantSkills)
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
