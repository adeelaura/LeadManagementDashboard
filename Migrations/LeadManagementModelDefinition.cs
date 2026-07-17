using LeadManagement.Web.Data.Configurations;
using LeadManagement.Web.Data.Seed;
using Microsoft.EntityFrameworkCore;

namespace LeadManagement.Web.Migrations;

internal static class LeadManagementModelDefinition
{
    public static void Build(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.10");
        modelBuilder.ApplyConfiguration(new LeadStatusConfiguration());
        modelBuilder.ApplyConfiguration(new LeadConfiguration());
        modelBuilder.ApplyConfiguration(new LeadActivityConfiguration());
        SeedData.Apply(modelBuilder);
    }
}
