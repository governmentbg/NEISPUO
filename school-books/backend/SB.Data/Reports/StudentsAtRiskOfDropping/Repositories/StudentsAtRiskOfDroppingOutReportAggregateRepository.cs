namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Domain;
internal class StudentsAtRiskOfDroppingOutReportAggregateRepository
    : ScopedAggregateRepository<StudentsAtRiskOfDroppingOutReport>, IStudentsAtRiskOfDroppingOutReportAggregateRepository
{
    public StudentsAtRiskOfDroppingOutReportAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<StudentsAtRiskOfDroppingOutReport>, IQueryable<StudentsAtRiskOfDroppingOutReport>>[] Includes =>
        new Func<IQueryable<StudentsAtRiskOfDroppingOutReport>, IQueryable<StudentsAtRiskOfDroppingOutReport>>[]
        {
            (q) => q.Include(e => e.Items),
        };

    public async Task RemoveAsync(int schoolYear, int studentsAtRiskOfDroppingOutReportId, CancellationToken ct)
    {
        await using var transaction = await this.DbContext.Database.BeginTransactionAsync(ct);

        await this.DbContext.Set<StudentsAtRiskOfDroppingOutReportItem>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.StudentsAtRiskOfDroppingOutReportId == studentsAtRiskOfDroppingOutReportId)
            .ExecuteDeleteAsync(ct);

        await this.DbContext.Set<StudentsAtRiskOfDroppingOutReport>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.StudentsAtRiskOfDroppingOutReportId == studentsAtRiskOfDroppingOutReportId)
            .ExecuteDeleteAsync(ct);

        await transaction.CommitAsync(ct);
    }
}
