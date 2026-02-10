namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class MissingTopicsReportsAggregateRepository : ScopedAggregateRepository<MissingTopicsReport>, IMissingTopicsReportsAggregateRepository
{
    public MissingTopicsReportsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<MissingTopicsReport>, IQueryable<MissingTopicsReport>>[] Includes =>
        new Func<IQueryable<MissingTopicsReport>, IQueryable<MissingTopicsReport>>[]
        {
            (q) => q.Include(e => e.Items).ThenInclude(sfy => sfy.Teachers),
        };

    public async Task RemoveAsync(int schoolYear, int missingTopicsReportId, CancellationToken ct)
    {
        await using var transaction = await this.DbContext.Database.BeginTransactionAsync(ct);

        await this.DbContext.Set<MissingTopicsReportItemTeacher>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.MissingTopicsReportId == missingTopicsReportId)
            .ExecuteDeleteAsync(ct);

        await this.DbContext.Set<MissingTopicsReportItem>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.MissingTopicsReportId == missingTopicsReportId)
            .ExecuteDeleteAsync(ct);

        await this.DbContext.Set<MissingTopicsReport>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.MissingTopicsReportId == missingTopicsReportId)
            .ExecuteDeleteAsync(ct);

        await transaction.CommitAsync(ct);
    }
}
