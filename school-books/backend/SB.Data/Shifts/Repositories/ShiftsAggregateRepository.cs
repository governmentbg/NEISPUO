namespace SB.Data;

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SB.Domain;

internal class ShiftsAggregateRepository : ScopedAggregateRepository<Shift>
{
    public ShiftsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<Shift>, IQueryable<Shift>>[] Includes =>
        new Func<IQueryable<Shift>, IQueryable<Shift>>[]
        {
            (q) => q.Include(e => e.Hours)
        };
}
