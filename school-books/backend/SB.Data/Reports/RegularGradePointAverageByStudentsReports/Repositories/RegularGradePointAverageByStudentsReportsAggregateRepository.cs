namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Domain;

class RegularGradePointAverageByStudentsReportsAggregateRepository : ScopedAggregateRepository<RegularGradePointAverageByStudentsReport>, IRegularGradePointAverageByStudentsReportsAggregateRepository
{
    public RegularGradePointAverageByStudentsReportsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<RegularGradePointAverageByStudentsReport>, IQueryable<RegularGradePointAverageByStudentsReport>>[] Includes =>
        new Func<IQueryable<RegularGradePointAverageByStudentsReport>, IQueryable<RegularGradePointAverageByStudentsReport>>[]
        {
            (q) => q.Include(e => e.Items)
        };

    public async Task RemoveAsync(int schoolYear, int regularGradePointAverageByStudentsId, CancellationToken ct)
    {
        await using var transaction = await this.DbContext.Database.BeginTransactionAsync(ct);

        await this.DbContext.Set<RegularGradePointAverageByStudentsReportItem>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.RegularGradePointAverageByStudentsReportId == regularGradePointAverageByStudentsId)
            .ExecuteDeleteAsync(ct);

        await this.DbContext.Set<RegularGradePointAverageByStudentsReport>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.RegularGradePointAverageByStudentsReportId == regularGradePointAverageByStudentsId)
            .ExecuteDeleteAsync(ct);

        await transaction.CommitAsync(ct);
    }
}
