namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;

internal class NotesAggregateRepository : ScopedAggregateRepository<Note>
{
    public NotesAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<Note>, IQueryable<Note>>[] Includes =>
        new Func<IQueryable<Note>, IQueryable<Note>>[]
        {
            (q) => q.Include(s => s.Students)
        };
}
