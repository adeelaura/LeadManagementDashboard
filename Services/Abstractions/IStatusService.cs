using LeadManagement.Web.Services.Models;
using LeadManagement.Web.ViewModels;

namespace LeadManagement.Web.Services.Abstractions;

public interface IStatusService
{
    Task<IReadOnlyList<StatusOptionViewModel>> GetActiveOptionsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StatusListItemViewModel>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<StatusEditorViewModel?> GetForEditAsync(
        int statusId,
        CancellationToken cancellationToken = default);

    Task<OperationResult<int>> CreateAsync(
        StatusEditorViewModel model,
        CancellationToken cancellationToken = default);

    Task<OperationResult> UpdateAsync(
        int statusId,
        StatusEditorViewModel model,
        CancellationToken cancellationToken = default);

    Task<OperationResult> SetActiveAsync(
        int statusId,
        bool isActive,
        CancellationToken cancellationToken = default);
}
