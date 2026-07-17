using LeadManagement.Web.Data;
using LeadManagement.Web.Domain.Entities;
using LeadManagement.Web.Services.Abstractions;
using LeadManagement.Web.Services.Models;
using LeadManagement.Web.Utilities;
using LeadManagement.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LeadManagement.Web.Services;

public sealed class LeadService(
    LeadManagementDbContext dbContext,
    TimeProvider timeProvider,
    ILogger<LeadService> logger) : ILeadService
{
    private const string UserStatusChangeNote = "Status changed by user";

    private readonly LeadManagementDbContext _dbContext = dbContext;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly ILogger<LeadService> _logger = logger;

    public async Task<DashboardViewModel> GetDashboardAsync(
        string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        var statuses = await _dbContext.Statuses
            .AsNoTracking()
            .Where(status => status.IsActive)
            .OrderBy(status => status.DisplayOrder)
            .ThenBy(status => status.Id)
            .Select(status => new StatusProjection
            {
                Id = status.Id,
                Name = status.Name,
                DisplayOrder = status.DisplayOrder,
                ColorCode = status.ColorCode
            })
            .ToListAsync(cancellationToken);

        var totalLeads = await _dbContext.Leads
            .AsNoTracking()
            .CountAsync(lead => lead.Status.IsActive, cancellationToken);

        var normalizedSearch = searchTerm?.Trim();
        var leadsQuery = _dbContext.Leads
            .AsNoTracking()
            .Where(lead => lead.Status.IsActive);

        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            leadsQuery = leadsQuery.Where(lead =>
                lead.FirstName.Contains(normalizedSearch) ||
                lead.LastName.Contains(normalizedSearch) ||
                lead.Email.Contains(normalizedSearch) ||
                lead.Phone.Contains(normalizedSearch) ||
                lead.Company.Contains(normalizedSearch));
        }

        var leads = await leadsQuery
            .OrderBy(lead => lead.CreatedAt)
            .ThenBy(lead => lead.Id)
            .Select(lead => new LeadProjection
            {
                Id = lead.Id,
                FirstName = lead.FirstName,
                LastName = lead.LastName,
                Email = lead.Email,
                Phone = lead.Phone,
                Company = lead.Company,
                CreatedAt = lead.CreatedAt,
                StatusId = lead.StatusId
            })
            .ToListAsync(cancellationToken);

        var columns = statuses
            .Select((status, index) => new StatusColumnViewModel
            {
                Id = status.Id,
                Name = status.Name,
                DisplayOrder = status.DisplayOrder,
                ColorCode = ColorUtility.NormalizeOrDefault(status.ColorCode),
                ContrastTextColor = ColorUtility.GetContrastTextColor(status.ColorCode),
                Leads = leads
                    .Where(lead => lead.StatusId == status.Id)
                    .Select(lead => new LeadCardViewModel
                    {
                        Id = lead.Id,
                        FullName = $"{lead.FirstName} {lead.LastName}".Trim(),
                        Email = lead.Email,
                        Phone = lead.Phone,
                        Company = lead.Company,
                        CreatedAt = lead.CreatedAt,
                        StatusId = lead.StatusId,
                        CanMoveBackward = index > 0,
                        CanMoveForward = index < statuses.Count - 1
                    })
                    .ToList()
            })
            .ToList();

        return new DashboardViewModel
        {
            Statuses = columns,
            SearchTerm = normalizedSearch,
            TotalLeads = totalLeads,
            VisibleLeads = leads.Count
        };
    }

    public async Task<LeadDetailsViewModel?> GetDetailsAsync(
        int leadId,
        CancellationToken cancellationToken = default)
    {
        var lead = await _dbContext.Leads
            .AsNoTracking()
            .Where(item => item.Id == leadId)
            .Select(item => new LeadDetailsProjection
            {
                Id = item.Id,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Email = item.Email,
                Phone = item.Phone,
                Company = item.Company,
                CreatedAt = item.CreatedAt,
                StatusId = item.StatusId,
                StatusName = item.Status.Name,
                StatusColorCode = item.Status.ColorCode
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (lead is null)
        {
            return null;
        }

        var activities = await _dbContext.LeadActivities
            .AsNoTracking()
            .Where(activity => activity.LeadId == leadId)
            .OrderByDescending(activity => activity.ChangedAt)
            .ThenByDescending(activity => activity.Id)
            .Select(activity => new LeadActivityViewModel
            {
                Id = activity.Id,
                FromStatusName = activity.FromStatus.Name,
                ToStatusName = activity.ToStatus.Name,
                FromStatusColorCode = activity.FromStatus.ColorCode,
                ToStatusColorCode = activity.ToStatus.ColorCode,
                ChangedAt = activity.ChangedAt,
                Note = activity.Note
            })
            .ToListAsync(cancellationToken);

        return new LeadDetailsViewModel
        {
            Id = lead.Id,
            FirstName = lead.FirstName,
            LastName = lead.LastName,
            Email = lead.Email,
            Phone = lead.Phone,
            Company = lead.Company,
            CreatedAt = lead.CreatedAt,
            StatusId = lead.StatusId,
            StatusName = lead.StatusName,
            StatusColorCode = ColorUtility.NormalizeOrDefault(lead.StatusColorCode),
            StatusContrastTextColor = ColorUtility.GetContrastTextColor(lead.StatusColorCode),
            ActivityHistory = activities
        };
    }

    public async Task<LeadEditorViewModel?> GetForEditAsync(
        int leadId,
        CancellationToken cancellationToken = default)
    {
        var lead = await _dbContext.Leads
            .AsNoTracking()
            .Where(item => item.Id == leadId)
            .Select(item => new
            {
                item.FirstName,
                item.LastName,
                item.Email,
                item.Phone,
                item.Company,
                item.StatusId,
                item.RowVersion
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (lead is null)
        {
            return null;
        }

        return new LeadEditorViewModel
        {
            FirstName = lead.FirstName,
            LastName = lead.LastName,
            Email = lead.Email,
            Phone = lead.Phone,
            Company = lead.Company,
            StatusId = lead.StatusId,
            RowVersion = Convert.ToBase64String(lead.RowVersion)
        };
    }

    public async Task<OperationResult<int>> CreateAsync(
        LeadEditorViewModel model,
        CancellationToken cancellationToken = default)
    {
        var validation = ValidateEditor(model);
        if (validation is not null)
        {
            return OperationResult<int>.Failure(validation);
        }

        var statusExists = await _dbContext.Statuses
            .AsNoTracking()
            .AnyAsync(status => status.Id == model.StatusId && status.IsActive, cancellationToken);

        if (!statusExists)
        {
            return OperationResult<int>.Failure("The selected status is unavailable.");
        }

        var lead = new Lead
        {
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            Email = model.Email.Trim(),
            Phone = model.Phone.Trim(),
            Company = model.Company.Trim(),
            StatusId = model.StatusId,
            CreatedAt = _timeProvider.GetUtcNow()
        };

        _dbContext.Leads.Add(lead);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return OperationResult<int>.Success(lead.Id, $"Lead {lead.FirstName} {lead.LastName} was created.");
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "Unable to create lead for {Email}.", lead.Email);
            return OperationResult<int>.Failure("The lead could not be created. Please try again.");
        }
    }

    public async Task<OperationResult> UpdateAsync(
        int leadId,
        LeadEditorViewModel model,
        CancellationToken cancellationToken = default)
    {
        var validation = ValidateEditor(model);
        if (validation is not null)
        {
            return OperationResult.Failure(validation);
        }

        var lead = await _dbContext.Leads
            .SingleOrDefaultAsync(item => item.Id == leadId, cancellationToken);

        if (lead is null)
        {
            return OperationResult.Failure("Lead not found.");
        }

        var targetStatus = await _dbContext.Statuses
            .AsNoTracking()
            .SingleOrDefaultAsync(
                status => status.Id == model.StatusId && status.IsActive,
                cancellationToken);

        if (targetStatus is null)
        {
            return OperationResult.Failure("The selected status is unavailable.");
        }

        if (!string.IsNullOrWhiteSpace(model.RowVersion))
        {
            try
            {
                _dbContext.Entry(lead).Property(item => item.RowVersion).OriginalValue =
                    Convert.FromBase64String(model.RowVersion);
            }
            catch (FormatException)
            {
                return OperationResult.Failure("The edit token is invalid. Reload the page and try again.");
            }
        }

        var previousStatusId = lead.StatusId;

        lead.FirstName = model.FirstName.Trim();
        lead.LastName = model.LastName.Trim();
        lead.Email = model.Email.Trim();
        lead.Phone = model.Phone.Trim();
        lead.Company = model.Company.Trim();
        lead.StatusId = model.StatusId;

        if (previousStatusId != model.StatusId)
        {
            _dbContext.LeadActivities.Add(new LeadActivity
            {
                LeadId = lead.Id,
                FromStatusId = previousStatusId,
                ToStatusId = model.StatusId,
                ChangedAt = _timeProvider.GetUtcNow(),
                Note = UserStatusChangeNote
            });
        }

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return OperationResult.Success($"Lead {lead.FirstName} {lead.LastName} was updated.");
        }
        catch (DbUpdateConcurrencyException exception)
        {
            _logger.LogWarning(exception, "Concurrency conflict while updating lead {LeadId}.", leadId);
            return OperationResult.Failure("This lead was changed by another user. Reload and try again.");
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "Unable to update lead {LeadId}.", leadId);
            return OperationResult.Failure("The lead could not be updated. Please try again.");
        }
    }

    public async Task<OperationResult> MoveAsync(
        int leadId,
        MoveDirection direction,
        CancellationToken cancellationToken = default)
    {
        var lead = await _dbContext.Leads
            .SingleOrDefaultAsync(item => item.Id == leadId, cancellationToken);

        if (lead is null)
        {
            return OperationResult.Failure("Lead not found.");
        }

        var statuses = await _dbContext.Statuses
            .AsNoTracking()
            .Where(status => status.IsActive)
            .OrderBy(status => status.DisplayOrder)
            .ThenBy(status => status.Id)
            .Select(status => new StatusProjection
            {
                Id = status.Id,
                Name = status.Name,
                DisplayOrder = status.DisplayOrder,
                ColorCode = status.ColorCode
            })
            .ToListAsync(cancellationToken);

        var currentIndex = statuses.FindIndex(status => status.Id == lead.StatusId);
        if (currentIndex < 0)
        {
            return OperationResult.Failure("The lead's current status is inactive or missing.");
        }

        var targetIndex = currentIndex + (int)direction;
        if (targetIndex < 0)
        {
            return OperationResult.Failure("This lead is already in the first status.");
        }

        if (targetIndex >= statuses.Count)
        {
            return OperationResult.Failure("This lead is already in the last status.");
        }

        var currentStatus = statuses[currentIndex];
        var targetStatus = statuses[targetIndex];

        lead.StatusId = targetStatus.Id;
        _dbContext.LeadActivities.Add(new LeadActivity
        {
            LeadId = lead.Id,
            FromStatusId = currentStatus.Id,
            ToStatusId = targetStatus.Id,
            ChangedAt = _timeProvider.GetUtcNow(),
            Note = UserStatusChangeNote
        });

        try
        {
            // EF Core wraps this update and activity insert in one database transaction.
            await _dbContext.SaveChangesAsync(cancellationToken);
            return OperationResult.Success(
                $"{lead.FirstName} {lead.LastName} moved from {currentStatus.Name} to {targetStatus.Name}.");
        }
        catch (DbUpdateConcurrencyException exception)
        {
            _logger.LogWarning(exception, "Concurrency conflict while moving lead {LeadId}.", leadId);
            return OperationResult.Failure("This lead was changed by another user. Refresh and try again.");
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "Unable to move lead {LeadId}.", leadId);
            return OperationResult.Failure("The lead status could not be changed. Please try again.");
        }
    }

    private static string? ValidateEditor(LeadEditorViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.FirstName) ||
            string.IsNullOrWhiteSpace(model.LastName))
        {
            return "First name and last name are required.";
        }

        if (string.IsNullOrWhiteSpace(model.Email))
        {
            return "Email is required.";
        }

        if (string.IsNullOrWhiteSpace(model.Phone))
        {
            return "Phone is required.";
        }

        if (string.IsNullOrWhiteSpace(model.Company))
        {
            return "Company is required.";
        }

        if (model.StatusId < 1)
        {
            return "Select a valid status.";
        }

        return null;
    }

    private sealed class StatusProjection
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public int DisplayOrder { get; init; }
        public string ColorCode { get; init; } = string.Empty;
    }

    private sealed class LeadProjection
    {
        public int Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Phone { get; init; } = string.Empty;
        public string Company { get; init; } = string.Empty;
        public DateTimeOffset CreatedAt { get; init; }
        public int StatusId { get; init; }
    }

    private sealed class LeadDetailsProjection
    {
        public int Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Phone { get; init; } = string.Empty;
        public string Company { get; init; } = string.Empty;
        public DateTimeOffset CreatedAt { get; init; }
        public int StatusId { get; init; }
        public string StatusName { get; init; } = string.Empty;
        public string StatusColorCode { get; init; } = string.Empty;
    }
}
