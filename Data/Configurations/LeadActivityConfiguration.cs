using LeadManagement.Web.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeadManagement.Web.Data.Configurations;

public sealed class LeadActivityConfiguration : IEntityTypeConfiguration<LeadActivity>
{
    public void Configure(EntityTypeBuilder<LeadActivity> builder)
    {
        builder.ToTable("LeadActivities");

        builder.HasKey(activity => activity.Id);

        builder.Property(activity => activity.ChangedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();

        builder.Property(activity => activity.Note)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasOne(activity => activity.Lead)
            .WithMany(lead => lead.Activities)
            .HasForeignKey(activity => activity.LeadId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(activity => activity.FromStatus)
            .WithMany(status => status.ActivitiesFrom)
            .HasForeignKey(activity => activity.FromStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(activity => activity.ToStatus)
            .WithMany(status => status.ActivitiesTo)
            .HasForeignKey(activity => activity.ToStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(activity => new { activity.LeadId, activity.ChangedAt });
        builder.HasIndex(activity => activity.FromStatusId);
        builder.HasIndex(activity => activity.ToStatusId);
    }
}
