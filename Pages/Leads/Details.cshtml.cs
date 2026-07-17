using LeadManagement.Web.Services.Abstractions;
using LeadManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LeadManagement.Web.Pages.Leads;

public sealed class DetailsModel(ILeadService leadService) : PageModel
{
    private readonly ILeadService _leadService = leadService;

    public LeadDetailsViewModel Lead { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        var lead = await _leadService.GetDetailsAsync(id, cancellationToken);
        if (lead is null)
        {
            return NotFound();
        }

        Lead = lead;
        return Page();
    }
}
