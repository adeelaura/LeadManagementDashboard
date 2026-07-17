namespace LeadManagement.Web.Domain.Entities;

public sealed class LeadStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public string ColorCode { get; set; } = "#0D6EFD";

    public bool IsActive { get; set; } = true;

    public ICollection<Lead> Leads { get; } = new List<Lead>();

    public ICollection<LeadActivity> ActivitiesFrom { get; } = new List<LeadActivity>();

    public ICollection<LeadActivity> ActivitiesTo { get; } = new List<LeadActivity>();
}
