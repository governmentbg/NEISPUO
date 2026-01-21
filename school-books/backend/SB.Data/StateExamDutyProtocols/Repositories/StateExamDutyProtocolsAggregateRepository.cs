namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;

internal class StateExamDutyProtocolsAggregateRepository : ScopedAggregateRepository<StateExamDutyProtocol>
{
    public StateExamDutyProtocolsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<StateExamDutyProtocol>, IQueryable<StateExamDutyProtocol>>[] Includes =>
        new Func<IQueryable<StateExamDutyProtocol>, IQueryable<StateExamDutyProtocol>>[]
        {
            (q) => q.Include(e => e.Supervisors)
        };
}
