using LeadManagement.Web.Services.Abstractions;
using LeadManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LeadManagement.Web.Pages.Statuses;

public sealed class EditModel(IStatusService statusService) : PageModel
{
    private readonly IStatusService _statusService = statusService;

    [BindProperty]
    public StatusEditorViewModel Status { get; set; } = new();

    public int StatusId { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        var status = await _statusService.GetForEditAsync(id, cancellationToken);
        if (status is null)
        {
            return NotFound();
        }

        StatusId = id;
        Status = status;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        StatusId = id;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _statusService.UpdateAsync(id, Status, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return Page();
        }

        TempData["ToastMessage"] = result.Message;
        TempData["ToastType"] = "success";
        return RedirectToPage("/Statuses/Index");
    }
}
