using System.ComponentModel.DataAnnotations;

namespace LeadManagement.Web.ViewModels;

public sealed class LeadEditorViewModel
{
    [Required]
    [StringLength(100)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(40)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [StringLength(160)]
    public string Company { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Select a valid status.")]
    [Display(Name = "Status")]
    public int StatusId { get; set; }

    public string? RowVersion { get; set; }
}

public sealed class StatusOptionViewModel
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string ColorCode { get; init; } = "#0D6EFD";
}
