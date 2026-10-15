using Kursach.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kursach.Web.Data.Configurations;

/// <summary>
/// Конфигурация сущности <see cref="Company"/>.
/// </summary>
public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Phone).IsRequired().HasMaxLength(30);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Address).HasMaxLength(250);
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.IndustryId);

        builder.HasOne(x => x.Industry)
            .WithMany(x => x.Companies)
            .HasForeignKey(x => x.IndustryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
