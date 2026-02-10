namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class GradelessStudentsReportsAggregateRepository : ScopedAggregateRepository<GradelessStudentsReport>, IGradelessStudentsReportsAggregateRepository
{
    public GradelessStudentsReportsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<GradelessStudentsReport>, IQueryable<GradelessStudentsReport>>[] Includes =>
        new Func<IQueryable<GradelessStudentsReport>, IQueryable<GradelessStudentsReport>>[]
        {
            (q) => q.Include(e => e.Items)
        };

    public async Task RemoveAsync(int schoolYear, int gradelessStudentsId, CancellationToken ct)
    {
        await using var transaction = await this.DbContext.Database.BeginTransactionAsync(ct);

        await this.DbContext.Set<GradelessStudentsReportItem>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.GradelessStudentsReportId == gradelessStudentsId)
            .ExecuteDeleteAsync(ct);

        await this.DbContext.Set<GradelessStudentsReport>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.GradelessStudentsReportId == gradelessStudentsId)
            .ExecuteDeleteAsync(ct);

        await transaction.CommitAsync(ct);
    }
}
