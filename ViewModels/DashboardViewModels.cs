namespace LeadManagement.Web.ViewModels;

public sealed class DashboardViewModel
{
    public IReadOnlyList<StatusColumnViewModel> Statuses { get; init; } = [];

    public string? SearchTerm { get; init; }

    public int TotalLeads { get; init; }

    public int VisibleLeads { get; init; }

    public int ActiveStatusCount => Statuses.Count;
}

public sealed class StatusColumnViewModel
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public int DisplayOrder { get; init; }

    public string ColorCode { get; init; } = "#0D6EFD";

    public string ContrastTextColor { get; init; } = "#FFFFFF";

    public IReadOnlyList<LeadCardViewModel> Leads { get; init; } = [];
}

public sealed class LeadCardViewModel
{
    public int Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Phone { get; init; } = string.Empty;

    public string Company { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }

    public int StatusId { get; init; }

    public bool CanMoveBackward { get; init; }

    public bool CanMoveForward { get; init; }
}
