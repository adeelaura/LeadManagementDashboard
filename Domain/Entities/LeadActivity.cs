namespace LeadManagement.Web.Domain.Entities;

public sealed class LeadActivity
{
    public int Id { get; set; }

    public int LeadId { get; set; }

    public int FromStatusId { get; set; }

    public int ToStatusId { get; set; }

    public DateTimeOffset ChangedAt { get; set; }

    public string Note { get; set; } = string.Empty;

    public Lead Lead { get; set; } = null!;

    public LeadStatus FromStatus { get; set; } = null!;

    public LeadStatus ToStatus { get; set; } = null!;
}
