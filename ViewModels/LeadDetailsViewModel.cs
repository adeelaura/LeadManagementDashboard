namespace LeadManagement.Web.ViewModels;

public sealed class LeadDetailsViewModel
{
    public int Id { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}".Trim();

    public string Email { get; init; } = string.Empty;

    public string Phone { get; init; } = string.Empty;

    public string Company { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }

    public int StatusId { get; init; }

    public string StatusName { get; init; } = string.Empty;

    public string StatusColorCode { get; init; } = "#0D6EFD";

    public string StatusContrastTextColor { get; init; } = "#FFFFFF";

    public IReadOnlyList<LeadActivityViewModel> ActivityHistory { get; init; } = [];
}

public sealed class LeadActivityViewModel
{
    public int Id { get; init; }

    public string FromStatusName { get; init; } = string.Empty;

    public string ToStatusName { get; init; } = string.Empty;

    public string FromStatusColorCode { get; init; } = "#6C757D";

    public string ToStatusColorCode { get; init; } = "#0D6EFD";

    public DateTimeOffset ChangedAt { get; init; }

    public string Note { get; init; } = string.Empty;
}
