using Microsoft.EntityFrameworkCore;

namespace LeadManagement.Web.Data;

public static class DatabaseInitializer
{
    public static async Task ApplyMigrationsAsync(
        IServiceProvider serviceProvider,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LeadManagementDbContext>();

        logger.LogInformation("Applying pending database migrations.");
        try
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
            logger.LogInformation("Database migrations applied successfully.");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("PendingModelChangesWarning") || ex.Message.Contains("pending changes"))
        {
            logger.LogWarning(ex, "Database migrations were not applied due to pending model changes. Create a new migration or set Database:ApplyMigrationsOnStartup to false.");
        }
    }
}
