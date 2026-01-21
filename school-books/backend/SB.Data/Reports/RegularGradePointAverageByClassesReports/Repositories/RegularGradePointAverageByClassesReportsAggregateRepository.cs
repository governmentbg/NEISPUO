namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Domain;

class RegularGradePointAverageByClassesReportsAggregateRepository : ScopedAggregateRepository<RegularGradePointAverageByClassesReport>, IRegularGradePointAverageByClassesReportsAggregateRepository
{
    public RegularGradePointAverageByClassesReportsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<RegularGradePointAverageByClassesReport>, IQueryable<RegularGradePointAverageByClassesReport>>[] Includes =>
        new Func<IQueryable<RegularGradePointAverageByClassesReport>, IQueryable<RegularGradePointAverageByClassesReport>>[]
        {
            (q) => q.Include(e => e.Items)
        };

    public async Task RemoveAsync(int schoolYear, int regularGradePointAverageByClassesId, CancellationToken ct)
    {
        await using var transaction = await this.DbContext.Database.BeginTransactionAsync(ct);

        await this.DbContext.Set<RegularGradePointAverageByClassesReportItem>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.RegularGradePointAverageByClassesReportId == regularGradePointAverageByClassesId)
            .ExecuteDeleteAsync(ct);

        await this.DbContext.Set<RegularGradePointAverageByClassesReport>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.RegularGradePointAverageByClassesReportId == regularGradePointAverageByClassesId)
            .ExecuteDeleteAsync(ct);

        await transaction.CommitAsync(ct);
    }
}
