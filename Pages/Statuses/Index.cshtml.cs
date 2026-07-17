using LeadManagement.Web.Services.Abstractions;
using LeadManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LeadManagement.Web.Pages.Statuses;

public sealed class IndexModel(IStatusService statusService) : PageModel
{
    private readonly IStatusService _statusService = statusService;

    public IReadOnlyList<StatusListItemViewModel> Statuses { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Statuses = await _statusService.GetAllAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostToggleAsync(
        int id,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var result = await _statusService.SetActiveAsync(id, isActive, cancellationToken);
        TempData["ToastMessage"] = result.Message;
        TempData["ToastType"] = result.Succeeded ? "success" : "warning";
        return RedirectToPage();
    }
}
