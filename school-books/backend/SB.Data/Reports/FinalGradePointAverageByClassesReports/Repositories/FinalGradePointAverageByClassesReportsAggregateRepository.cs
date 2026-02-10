namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Domain;

class FinalGradePointAverageByClassesReportsAggregateRepository : ScopedAggregateRepository<FinalGradePointAverageByClassesReport>, IFinalGradePointAverageByClassesReportsAggregateRepository
{
    public FinalGradePointAverageByClassesReportsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<FinalGradePointAverageByClassesReport>, IQueryable<FinalGradePointAverageByClassesReport>>[] Includes =>
        new Func<IQueryable<FinalGradePointAverageByClassesReport>, IQueryable<FinalGradePointAverageByClassesReport>>[]
        {
            (q) => q.Include(e => e.Items)
        };

    public async Task RemoveAsync(int schoolYear, int finalGradePointAverageByClassesId, CancellationToken ct)
    {
        await using var transaction = await this.DbContext.Database.BeginTransactionAsync(ct);

        await this.DbContext.Set<FinalGradePointAverageByClassesReportItem>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.FinalGradePointAverageByClassesReportId == finalGradePointAverageByClassesId)
            .ExecuteDeleteAsync(ct);

        await this.DbContext.Set<FinalGradePointAverageByClassesReport>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.FinalGradePointAverageByClassesReportId == finalGradePointAverageByClassesId)
            .ExecuteDeleteAsync(ct);

        await transaction.CommitAsync(ct);
    }
}
