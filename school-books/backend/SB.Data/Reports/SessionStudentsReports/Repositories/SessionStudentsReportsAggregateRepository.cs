namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Domain;
internal class SessionStudentsReportsAggregateRepository
    : ScopedAggregateRepository<SessionStudentsReport>, ISessionStudentsReportsAggregateRepository
{
    public SessionStudentsReportsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<SessionStudentsReport>, IQueryable<SessionStudentsReport>>[] Includes =>
        new Func<IQueryable<SessionStudentsReport>, IQueryable<SessionStudentsReport>>[]
        {
            (q) => q.Include(e => e.Items),
        };

    public async Task RemoveAsync(int schoolYear, int sessionStudentsReportId, CancellationToken ct)
    {
        await using var transaction = await this.DbContext.Database.BeginTransactionAsync(ct);

        await this.DbContext.Set<SessionStudentsReportItem>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.SessionStudentsReportId == sessionStudentsReportId)
            .ExecuteDeleteAsync(ct);

        await this.DbContext.Set<SessionStudentsReport>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.SessionStudentsReportId == sessionStudentsReportId)
            .ExecuteDeleteAsync(ct);

        await transaction.CommitAsync(ct);
    }
}
