namespace LeadManagement.Web.Domain.Entities;

public sealed class Lead
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Company { get; set; } = string.Empty;

    public int StatusId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public byte[] RowVersion { get; set; } = [];

    public LeadStatus Status { get; set; } = null!;

    public ICollection<LeadActivity> Activities { get; } = new List<LeadActivity>();
}
