using LeadManagement.Web.Data;
using LeadManagement.Web.Domain.Entities;
using LeadManagement.Web.Services.Abstractions;
using LeadManagement.Web.Services.Models;
using LeadManagement.Web.Utilities;
using LeadManagement.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LeadManagement.Web.Services;

public sealed class StatusService(
    LeadManagementDbContext dbContext,
    ILogger<StatusService> logger) : IStatusService
{
    private readonly LeadManagementDbContext _dbContext = dbContext;
    private readonly ILogger<StatusService> _logger = logger;

    public async Task<IReadOnlyList<StatusOptionViewModel>> GetActiveOptionsAsync(
        CancellationToken cancellationToken = default) =>
        await _dbContext.Statuses
            .AsNoTracking()
            .Where(status => status.IsActive)
            .OrderBy(status => status.DisplayOrder)
            .ThenBy(status => status.Id)
            .Select(status => new StatusOptionViewModel
            {
                Id = status.Id,
                Name = status.Name,
                ColorCode = status.ColorCode
            })
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<StatusListItemViewModel>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var statuses = await _dbContext.Statuses
            .AsNoTracking()
            .OrderBy(status => status.DisplayOrder)
            .ThenBy(status => status.Id)
            .Select(status => new
            {
                status.Id,
                status.Name,
                status.DisplayOrder,
                status.ColorCode,
                status.IsActive,
                LeadCount = status.Leads.Count
            })
            .ToListAsync(cancellationToken);

        return statuses.Select(status => new StatusListItemViewModel
        {
            Id = status.Id,
            Name = status.Name,
            DisplayOrder = status.DisplayOrder,
            ColorCode = ColorUtility.NormalizeOrDefault(status.ColorCode),
            ContrastTextColor = ColorUtility.GetContrastTextColor(status.ColorCode),
            IsActive = status.IsActive,
            LeadCount = status.LeadCount
        }).ToList();
    }

    public async Task<StatusEditorViewModel?> GetForEditAsync(
        int statusId,
        CancellationToken cancellationToken = default) =>
        await _dbContext.Statuses
            .AsNoTracking()
            .Where(status => status.Id == statusId)
            .Select(status => new StatusEditorViewModel
            {
                Name = status.Name,
                DisplayOrder = status.DisplayOrder,
                ColorCode = status.ColorCode,
                IsActive = status.IsActive
            })
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<OperationResult<int>> CreateAsync(
        StatusEditorViewModel model,
        CancellationToken cancellationToken = default)
    {
        var validation = Validate(model);
        if (validation is not null)
        {
            return OperationResult<int>.Failure(validation);
        }

        var normalizedName = model.Name.Trim();
        var normalizedNameUpper = normalizedName.ToUpperInvariant();
        var duplicateExists = await _dbContext.Statuses
            .AsNoTracking()
            .AnyAsync(
                status => status.Name.ToUpper() == normalizedNameUpper,
                cancellationToken);

        if (duplicateExists)
        {
            return OperationResult<int>.Failure("A status with this name already exists.");
        }

        var status = new LeadStatus
        {
            Name = normalizedName,
            DisplayOrder = model.DisplayOrder,
            ColorCode = ColorUtility.NormalizeOrDefault(model.ColorCode),
            IsActive = model.IsActive
        };

        _dbContext.Statuses.Add(status);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return OperationResult<int>.Success(status.Id, $"Status {status.Name} was created.");
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "Unable to create status {StatusName}.", status.Name);
            return OperationResult<int>.Failure("The status could not be created. Please try again.");
        }
    }

    public async Task<OperationResult> UpdateAsync(
        int statusId,
        StatusEditorViewModel model,
        CancellationToken cancellationToken = default)
    {
        var validation = Validate(model);
        if (validation is not null)
        {
            return OperationResult.Failure(validation);
        }

        var status = await _dbContext.Statuses
            .SingleOrDefaultAsync(item => item.Id == statusId, cancellationToken);

        if (status is null)
        {
            return OperationResult.Failure("Status not found.");
        }

        var normalizedName = model.Name.Trim();
        var normalizedNameUpper = normalizedName.ToUpperInvariant();
        var duplicateExists = await _dbContext.Statuses
            .AsNoTracking()
            .AnyAsync(
                item => item.Id != statusId && item.Name.ToUpper() == normalizedNameUpper,
                cancellationToken);

        if (duplicateExists)
        {
            return OperationResult.Failure("A status with this name already exists.");
        }

        if (status.IsActive && !model.IsActive)
        {
            var deactivationCheck = await CanDeactivateAsync(statusId, cancellationToken);
            if (!deactivationCheck.Succeeded)
            {
                return deactivationCheck;
            }
        }

        status.Name = normalizedName;
        status.DisplayOrder = model.DisplayOrder;
        status.ColorCode = ColorUtility.NormalizeOrDefault(model.ColorCode);
        status.IsActive = model.IsActive;

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return OperationResult.Success($"Status {status.Name} was updated.");
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "Unable to update status {StatusId}.", statusId);
            return OperationResult.Failure("The status could not be updated. Please try again.");
        }
    }

    public async Task<OperationResult> SetActiveAsync(
        int statusId,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var status = await _dbContext.Statuses
            .SingleOrDefaultAsync(item => item.Id == statusId, cancellationToken);

        if (status is null)
        {
            return OperationResult.Failure("Status not found.");
        }

        if (status.IsActive == isActive)
        {
            return OperationResult.Success($"Status {status.Name} is already {(isActive ? "active" : "inactive")}.");
        }

        if (!isActive)
        {
            var deactivationCheck = await CanDeactivateAsync(statusId, cancellationToken);
            if (!deactivationCheck.Succeeded)
            {
                return deactivationCheck;
            }
        }

        status.IsActive = isActive;

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return OperationResult.Success(
                $"Status {status.Name} was {(isActive ? "activated" : "deactivated")}.");
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "Unable to change active state for status {StatusId}.", statusId);
            return OperationResult.Failure("The status could not be changed. Please try again.");
        }
    }

    private async Task<OperationResult> CanDeactivateAsync(
        int statusId,
        CancellationToken cancellationToken)
    {
        var hasLeads = await _dbContext.Leads
            .AsNoTracking()
            .AnyAsync(lead => lead.StatusId == statusId, cancellationToken);

        if (hasLeads)
        {
            return OperationResult.Failure(
                "Move all leads out of this status before deactivating it.");
        }

        var activeStatusCount = await _dbContext.Statuses
            .AsNoTracking()
            .CountAsync(status => status.IsActive, cancellationToken);

        if (activeStatusCount <= 1)
        {
            return OperationResult.Failure("At least one status must remain active.");
        }

        return OperationResult.Success("Status can be deactivated.");
    }

    private static string? Validate(StatusEditorViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
        {
            return "Status name is required.";
        }

        if (model.DisplayOrder < 1)
        {
            return "Display order must be greater than zero.";
        }

        if (!ColorUtility.IsValidHexColor(model.ColorCode))
        {
            return "Use a valid color in #RRGGBB format.";
        }

        return null;
    }
}
