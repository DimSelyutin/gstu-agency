using Kursach.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kursach.Web.Data.Configurations;

/// <summary>
/// Конфигурация сущности <see cref="VacancySkill"/>.
/// </summary>
public class VacancySkillConfiguration : IEntityTypeConfiguration<VacancySkill>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<VacancySkill> builder)
    {
        builder.ToTable("VacancySkills");
        builder.HasKey(x => new { x.VacancyId, x.SkillId });

        builder.HasOne(x => x.Vacancy)
            .WithMany(x => x.VacancySkills)
            .HasForeignKey(x => x.VacancyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Skill)
            .WithMany(x => x.VacancySkills)
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
