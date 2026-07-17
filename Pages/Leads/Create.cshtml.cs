using LeadManagement.Web.Services.Abstractions;
using LeadManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LeadManagement.Web.Pages.Leads;

public sealed class CreateModel(
    ILeadService leadService,
    IStatusService statusService) : PageModel
{
    private readonly ILeadService _leadService = leadService;
    private readonly IStatusService _statusService = statusService;

    [BindProperty]
    public LeadEditorViewModel Lead { get; set; } = new();

    public IReadOnlyList<StatusOptionViewModel> Statuses { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadStatusesAsync(cancellationToken);
        if (Statuses.Count == 0)
        {
            TempData["ToastMessage"] = "Create an active status before adding a lead.";
            TempData["ToastType"] = "warning";
            return RedirectToPage("/Statuses/Index");
        }

        Lead.StatusId = Statuses[0].Id;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadStatusesAsync(cancellationToken);
            return Page();
        }

        var result = await _leadService.CreateAsync(Lead, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await LoadStatusesAsync(cancellationToken);
            return Page();
        }

        TempData["ToastMessage"] = result.Message;
        TempData["ToastType"] = "success";
        return RedirectToPage("/Index");
    }

    private async Task LoadStatusesAsync(CancellationToken cancellationToken)
    {
        Statuses = await _statusService.GetActiveOptionsAsync(cancellationToken);
    }
}
