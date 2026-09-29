using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.WebApi;

public sealed class DatabaseHealthCheck(
    OrderDbContext dbContext,
    ILogger<DatabaseHealthCheck> logger) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.Usuarios.AsNoTracking().AnyAsync(cancellationToken);
            return HealthCheckResult.Healthy("Database is available.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Database readiness check failed.");
            return HealthCheckResult.Unhealthy("Database check failed.");
        }
    }
}