namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;

internal class PublicationsAggregateRepository : ScopedAggregateRepository<Publication>
{
    public PublicationsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<Publication>, IQueryable<Publication>>[] Includes =>
        new Func<IQueryable<Publication>, IQueryable<Publication>>[]
        {
            (q) => q.Include(e => e.Files)
        };
}
