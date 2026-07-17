using LeadManagement.Web.Services.Abstractions;
using LeadManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LeadManagement.Web.Pages.Statuses;

public sealed class CreateModel(IStatusService statusService) : PageModel
{
    private readonly IStatusService _statusService = statusService;

    [BindProperty]
    public StatusEditorViewModel Status { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _statusService.CreateAsync(Status, cancellationToken);
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
