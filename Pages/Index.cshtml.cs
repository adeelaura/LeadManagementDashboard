using LeadManagement.Web.Services.Abstractions;
using LeadManagement.Web.Services.Models;
using LeadManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LeadManagement.Web.Pages;

public sealed class IndexModel(ILeadService leadService) : PageModel
{
    private readonly ILeadService _leadService = leadService;

    [BindProperty(SupportsGet = true, Name = "q")]
    public string? SearchTerm { get; set; }

    public DashboardViewModel Dashboard { get; private set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Dashboard = await _leadService.GetDashboardAsync(SearchTerm, cancellationToken);
    }

    public async Task<IActionResult> OnPostMoveAsync(
        int leadId,
        string direction,
        string? q,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<MoveDirection>(direction, ignoreCase: true, out var parsedDirection) ||
            parsedDirection is not MoveDirection.Forward and not MoveDirection.Backward)
        {
            TempData["ToastMessage"] = "Invalid status movement requested.";
            TempData["ToastType"] = "danger";
            return RedirectToPage(new { q });
        }

        var result = await _leadService.MoveAsync(leadId, parsedDirection, cancellationToken);
        TempData["ToastMessage"] = result.Message;
        TempData["ToastType"] = result.Succeeded ? "success" : "warning";

        return RedirectToPage(new { q });
    }

    public async Task<IActionResult> OnGetLeadDetailsAsync(
        int leadId,
        CancellationToken cancellationToken)
    {
        var details = await _leadService.GetDetailsAsync(leadId, cancellationToken);
        return details is null ? NotFound() : Partial("_LeadDetails", details);
    }
}
