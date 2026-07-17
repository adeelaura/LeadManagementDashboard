using LeadManagement.Web.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeadManagement.Web.Data.Seed;

public static class SeedData
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LeadStatus>().HasData(
            new LeadStatus { Id = 1, Name = "New", DisplayOrder = 1, ColorCode = "#0D6EFD", IsActive = true },
            new LeadStatus { Id = 2, Name = "Contacted", DisplayOrder = 2, ColorCode = "#FFC107", IsActive = true },
            new LeadStatus { Id = 3, Name = "Qualified", DisplayOrder = 3, ColorCode = "#198754", IsActive = true },
            new LeadStatus { Id = 4, Name = "Closed", DisplayOrder = 4, ColorCode = "#6C757D", IsActive = true });

        modelBuilder.Entity<Lead>().HasData(
            new Lead
            {
                Id = 1,
                FirstName = "Aisha",
                LastName = "Khan",
                Email = "aisha.khan@example.com",
                Phone = "+971 50 555 0101",
                Company = "Northstar Digital",
                StatusId = 1,
                CreatedAt = Utc(2026, 6, 25, 8, 15)
            },
            new Lead
            {
                Id = 2,
                FirstName = "David",
                LastName = "Brown",
                Email = "david.brown@example.com",
                Phone = "+971 50 555 0102",
                Company = "BluePeak Logistics",
                StatusId = 2,
                CreatedAt = Utc(2026, 6, 24, 10, 30)
            },
            new Lead
            {
                Id = 3,
                FirstName = "Maria",
                LastName = "Garcia",
                Email = "maria.garcia@example.com",
                Phone = "+971 50 555 0103",
                Company = "Vertex Consulting",
                StatusId = 3,
                CreatedAt = Utc(2026, 6, 22, 13, 45)
            },
            new Lead
            {
                Id = 4,
                FirstName = "Omar",
                LastName = "Ali",
                Email = "omar.ali@example.com",
                Phone = "+971 50 555 0104",
                Company = "Crescent Holdings",
                StatusId = 4,
                CreatedAt = Utc(2026, 6, 18, 9, 0)
            },
            new Lead
            {
                Id = 5,
                FirstName = "Sophie",
                LastName = "Turner",
                Email = "sophie.turner@example.com",
                Phone = "+971 50 555 0105",
                Company = "Atlas Retail Group",
                StatusId = 1,
                CreatedAt = Utc(2026, 6, 27, 16, 20)
            });

        modelBuilder.Entity<LeadActivity>().HasData(
            new LeadActivity
            {
                Id = 1,
                LeadId = 2,
                FromStatusId = 1,
                ToStatusId = 2,
                ChangedAt = Utc(2026, 6, 26, 11, 0),
                Note = "Status changed by user"
            },
            new LeadActivity
            {
                Id = 2,
                LeadId = 3,
                FromStatusId = 1,
                ToStatusId = 2,
                ChangedAt = Utc(2026, 6, 23, 9, 30),
                Note = "Status changed by user"
            },
            new LeadActivity
            {
                Id = 3,
                LeadId = 3,
                FromStatusId = 2,
                ToStatusId = 3,
                ChangedAt = Utc(2026, 6, 24, 14, 10),
                Note = "Status changed by user"
            },
            new LeadActivity
            {
                Id = 4,
                LeadId = 4,
                FromStatusId = 3,
                ToStatusId = 4,
                ChangedAt = Utc(2026, 6, 21, 12, 0),
                Note = "Status changed by user"
            });
    }

    private static DateTimeOffset Utc(int year, int month, int day, int hour, int minute) =>
        new(year, month, day, hour, minute, 0, TimeSpan.Zero);
}
