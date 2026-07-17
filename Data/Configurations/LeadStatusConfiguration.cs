using LeadManagement.Web.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeadManagement.Web.Data.Configurations;

public sealed class LeadStatusConfiguration : IEntityTypeConfiguration<LeadStatus>
{
    public void Configure(EntityTypeBuilder<LeadStatus> builder)
    {
        builder.ToTable("Statuses");

        builder.HasKey(status => status.Id);

        builder.Property(status => status.Name)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(status => status.DisplayOrder)
            .IsRequired();

        builder.Property(status => status.ColorCode)
            .HasMaxLength(7)
            .IsUnicode(false)
            .HasDefaultValue("#0D6EFD")
            .IsRequired();

        builder.Property(status => status.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasIndex(status => status.Name)
            .IsUnique();

        builder.HasIndex(status => new { status.IsActive, status.DisplayOrder });
    }
}
