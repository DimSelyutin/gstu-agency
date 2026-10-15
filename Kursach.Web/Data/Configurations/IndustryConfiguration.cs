using Kursach.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kursach.Web.Data.Configurations;

/// <summary>
/// Конфигурация сущности <see cref="Industry"/>.
/// </summary>
public class IndustryConfiguration : IEntityTypeConfiguration<Industry>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Industry> builder)
    {
        builder.ToTable("Industries");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
