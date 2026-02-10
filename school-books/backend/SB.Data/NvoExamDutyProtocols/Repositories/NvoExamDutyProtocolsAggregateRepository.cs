namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;

internal class NvoExamDutyProtocolsAggregateRepository : ScopedAggregateRepository<NvoExamDutyProtocol>
{
    public NvoExamDutyProtocolsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<NvoExamDutyProtocol>, IQueryable<NvoExamDutyProtocol>>[] Includes =>
        new Func<IQueryable<NvoExamDutyProtocol>, IQueryable<NvoExamDutyProtocol>>[]
        {
            (q) => q.Include(e => e.Students),
            (q) => q.Include(e => e.Supervisors)
        };
}
