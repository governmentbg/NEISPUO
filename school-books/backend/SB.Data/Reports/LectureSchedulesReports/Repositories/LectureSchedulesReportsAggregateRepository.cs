namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class LectureSchedulesReportsAggregateRepository : ScopedAggregateRepository<LectureSchedulesReport>, ILectureSchedulesReportsAggregateRepository
{
    public LectureSchedulesReportsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<LectureSchedulesReport>, IQueryable<LectureSchedulesReport>>[] Includes =>
        new Func<IQueryable<LectureSchedulesReport>, IQueryable<LectureSchedulesReport>>[]
        {
            (q) => q.Include(e => e.Items),
        };

    public async Task RemoveAsync(int schoolYear, int lectureSchedulesReportId, CancellationToken ct)
    {
        await using var transaction = await this.DbContext.Database.BeginTransactionAsync(ct);

        await this.DbContext.Set<LectureSchedulesReportItem>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.LectureSchedulesReportId == lectureSchedulesReportId)
            .ExecuteDeleteAsync(ct);

        await this.DbContext.Set<LectureSchedulesReport>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.LectureSchedulesReportId == lectureSchedulesReportId)
            .ExecuteDeleteAsync(ct);

        await transaction.CommitAsync(ct);
    }
}
