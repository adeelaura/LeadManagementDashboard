using System.ComponentModel.DataAnnotations;

namespace LeadManagement.Web.ViewModels;

public sealed class StatusEditorViewModel
{
    [Required]
    [StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 9999)]
    [Display(Name = "Display order")]
    public int DisplayOrder { get; set; } = 1;

    [Required]
    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Use a valid hex color such as #0D6EFD.")]
    [StringLength(7, MinimumLength = 7)]
    [Display(Name = "Color")]
    public string ColorCode { get; set; } = "#0D6EFD";

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}

public sealed class StatusListItemViewModel
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public int DisplayOrder { get; init; }

    public string ColorCode { get; init; } = "#0D6EFD";

    public string ContrastTextColor { get; init; } = "#FFFFFF";

    public bool IsActive { get; init; }

    public int LeadCount { get; init; }
}
