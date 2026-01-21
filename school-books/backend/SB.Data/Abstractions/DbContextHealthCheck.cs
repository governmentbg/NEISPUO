namespace SB.Data;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

internal class DbContextHealthCheck : IHealthCheck
{
    private readonly DbContext dbContext;

    public DbContextHealthCheck(UnitOfWork unitOfWork)
    {
        if (unitOfWork == null)
        {
            throw new ArgumentNullException(nameof(unitOfWork));
        }

        this.dbContext = unitOfWork.DbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (await this.dbContext.Database.CanConnectAsync(ct))
        {
            return HealthCheckResult.Healthy();
        }

        return new HealthCheckResult(context.Registration.FailureStatus);
    }
}
