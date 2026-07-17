using LeadManagement.Web.Services.Models;
using LeadManagement.Web.ViewModels;

namespace LeadManagement.Web.Services.Abstractions;

public interface ILeadService
{
    Task<DashboardViewModel> GetDashboardAsync(string? searchTerm, CancellationToken cancellationToken = default);

    Task<LeadDetailsViewModel?> GetDetailsAsync(int leadId, CancellationToken cancellationToken = default);

    Task<LeadEditorViewModel?> GetForEditAsync(int leadId, CancellationToken cancellationToken = default);

    Task<OperationResult<int>> CreateAsync(
        LeadEditorViewModel model,
        CancellationToken cancellationToken = default);

    Task<OperationResult> UpdateAsync(
        int leadId,
        LeadEditorViewModel model,
        CancellationToken cancellationToken = default);

    Task<OperationResult> MoveAsync(
        int leadId,
        MoveDirection direction,
        CancellationToken cancellationToken = default);
}
