using LeadManagement.Web.Data.Seed;
using LeadManagement.Web.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeadManagement.Web.Data;

public sealed class LeadManagementDbContext(DbContextOptions<LeadManagementDbContext> options)
    : DbContext(options)
{
    public DbSet<LeadStatus> Statuses => Set<LeadStatus>();

    public DbSet<Lead> Leads => Set<Lead>();

    public DbSet<LeadActivity> LeadActivities => Set<LeadActivity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LeadManagementDbContext).Assembly);
        SeedData.Apply(modelBuilder);
    }
}
