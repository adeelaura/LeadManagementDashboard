using LeadManagement.Web.Data;
using LeadManagement.Web.Services;
using LeadManagement.Web.Services.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

builder.Services.AddDbContext<LeadManagementDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    });

    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
    }
});

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<ILeadService, LeadService>();
builder.Services.AddScoped<IStatusService, StatusService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

if (builder.Configuration.GetValue("HttpsRedirection:Enabled", true))
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    utc = TimeProvider.System.GetUtcNow()
})).ExcludeFromDescription();

if (builder.Configuration.GetValue("Database:ApplyMigrationsOnStartup", true))
{
    await DatabaseInitializer.ApplyMigrationsAsync(app.Services, app.Logger);
}

await app.RunAsync();

public partial class Program;
