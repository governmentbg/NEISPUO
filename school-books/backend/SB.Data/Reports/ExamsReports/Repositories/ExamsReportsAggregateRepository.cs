namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Domain;
internal class ExamsReportsAggregateRepository
    : ScopedAggregateRepository<ExamsReport>, IExamsReportsAggregateRepository
{
    public ExamsReportsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<ExamsReport>, IQueryable<ExamsReport>>[] Includes =>
        new Func<IQueryable<ExamsReport>, IQueryable<ExamsReport>>[]
        {
            (q) => q.Include(e => e.Items),
        };

    public async Task RemoveAsync(int schoolYear, int examsReportId, CancellationToken ct)
    {
        await using var transaction = await this.DbContext.Database.BeginTransactionAsync(ct);

        await this.DbContext.Set<ExamsReportItem>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.ExamsReportId == examsReportId)
            .ExecuteDeleteAsync(ct);

        await this.DbContext.Set<ExamsReport>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.ExamsReportId == examsReportId)
            .ExecuteDeleteAsync(ct);

        await transaction.CommitAsync(ct);
    }
}
