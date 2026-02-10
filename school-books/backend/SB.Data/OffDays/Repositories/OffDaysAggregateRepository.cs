namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class OffDaysAggregateRepository : ScopedAggregateRepository<OffDay>, IOffDaysAggregateRepository
{
    public OffDaysAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<OffDay>, IQueryable<OffDay>>[] Includes =>
        new Func<IQueryable<OffDay>, IQueryable<OffDay>>[]
        {
            (q) => q.Include(e => e.Classes),
            (q) => q.Include(e => e.ClassBooks)
        };

    public async Task<OffDay[]> FindAllByClassBookAsync(
        int schoolYear,
        int instId,
        int classBookId,
        CancellationToken ct)
    {
        return await this.FindEntitiesAsync(
            od =>
                od.SchoolYear == schoolYear &&
                od.InstId == instId &&
                od.ClassBooks.Any(odc =>
                    odc.SchoolYear == schoolYear &&
                    odc.ClassBookId == classBookId),
            ct);
    }
}
