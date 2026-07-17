using LeadManagement.Web.Services.Abstractions;
using LeadManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LeadManagement.Web.Pages.Leads;

public sealed class EditModel(
    ILeadService leadService,
    IStatusService statusService) : PageModel
{
    private readonly ILeadService _leadService = leadService;
    private readonly IStatusService _statusService = statusService;

    [BindProperty]
    public LeadEditorViewModel Lead { get; set; } = new();

    public IReadOnlyList<StatusOptionViewModel> Statuses { get; private set; } = [];

    public int LeadId { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        var lead = await _leadService.GetForEditAsync(id, cancellationToken);
        if (lead is null)
        {
            return NotFound();
        }

        LeadId = id;
        Lead = lead;
        await LoadStatusesAsync(cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        LeadId = id;

        if (!ModelState.IsValid)
        {
            await LoadStatusesAsync(cancellationToken);
            return Page();
        }

        var result = await _leadService.UpdateAsync(id, Lead, cancellationToken);
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
