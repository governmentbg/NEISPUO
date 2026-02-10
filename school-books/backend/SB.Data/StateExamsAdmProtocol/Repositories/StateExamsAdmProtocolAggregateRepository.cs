namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;

internal class StateExamsAdmProtocolAggregateRepository : ScopedAggregateRepository<StateExamsAdmProtocol>
{
    public StateExamsAdmProtocolAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<StateExamsAdmProtocol>, IQueryable<StateExamsAdmProtocol>>[] Includes =>
        new Func<IQueryable<StateExamsAdmProtocol>, IQueryable<StateExamsAdmProtocol>>[]
        {
            (q) => q.Include(e => e.Commissioners),
            (q) => q.Include(e => e.Students).ThenInclude(e => e.Subjects)
        };
}
