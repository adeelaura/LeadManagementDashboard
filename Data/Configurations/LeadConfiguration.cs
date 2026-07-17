using LeadManagement.Web.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeadManagement.Web.Data.Configurations;

public sealed class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.ToTable("Leads");

        builder.HasKey(lead => lead.Id);

        builder.Property(lead => lead.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(lead => lead.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(lead => lead.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(lead => lead.Phone)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(lead => lead.Company)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(lead => lead.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();

        builder.Property(lead => lead.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasOne(lead => lead.Status)
            .WithMany(status => status.Leads)
            .HasForeignKey(lead => lead.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(lead => lead.StatusId);
        builder.HasIndex(lead => lead.Email);
        builder.HasIndex(lead => lead.CreatedAt);
    }
}
