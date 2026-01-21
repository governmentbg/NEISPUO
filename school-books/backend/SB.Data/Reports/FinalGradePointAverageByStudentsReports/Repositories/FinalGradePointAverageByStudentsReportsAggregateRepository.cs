namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Domain;

class FinalGradePointAverageByStudentsReportsAggregateRepository : ScopedAggregateRepository<FinalGradePointAverageByStudentsReport>, IFinalGradePointAverageByStudentsReportsAggregateRepository
{
    public FinalGradePointAverageByStudentsReportsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<FinalGradePointAverageByStudentsReport>, IQueryable<FinalGradePointAverageByStudentsReport>>[] Includes =>
        new Func<IQueryable<FinalGradePointAverageByStudentsReport>, IQueryable<FinalGradePointAverageByStudentsReport>>[]
        {
            (q) => q.Include(e => e.Items)
        };

    public async Task RemoveAsync(int schoolYear, int finalGradePointAverageByStudentsId, CancellationToken ct)
    {
        await using var transaction = await this.DbContext.Database.BeginTransactionAsync(ct);

        await this.DbContext.Set<FinalGradePointAverageByStudentsReportItem>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.FinalGradePointAverageByStudentsReportId == finalGradePointAverageByStudentsId)
            .ExecuteDeleteAsync(ct);

        await this.DbContext.Set<FinalGradePointAverageByStudentsReport>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.FinalGradePointAverageByStudentsReportId == finalGradePointAverageByStudentsId)
            .ExecuteDeleteAsync(ct);

        await transaction.CommitAsync(ct);
    }
}
