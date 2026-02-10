namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;

internal class SupportsAggregateRepository : ScopedAggregateRepository<Support>
{
    public SupportsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<Support>, IQueryable<Support>>[] Includes =>
        new Func<IQueryable<Support>, IQueryable<Support>>[]
        {
            (q) => q.Include(s => s.Activities),
            (q) => q.Include(s => s.DifficultyTypes),
            (q) => q.Include(s => s.Teachers),
            (q) => q.Include(s => s.Students),
        };
}
