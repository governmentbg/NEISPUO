namespace SB.Data;

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SB.Domain;

internal class SpbsBookRecordsAggregateRepository : ScopedAggregateRepository<SpbsBookRecord>
{
    public SpbsBookRecordsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<SpbsBookRecord>, IQueryable<SpbsBookRecord>>[] Includes =>
        new Func<IQueryable<SpbsBookRecord>, IQueryable<SpbsBookRecord>>[]
        {
            (q) => q.Include(r => r.Movements),
            (q) => q.Include(r => r.Escapes),
            (q) => q.Include(r => r.Absences),
        };

}
