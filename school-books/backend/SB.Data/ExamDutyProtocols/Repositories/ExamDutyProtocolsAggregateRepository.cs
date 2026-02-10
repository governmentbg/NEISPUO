namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;

internal class ExamDutyProtocolsAggregateRepository : ScopedAggregateRepository<ExamDutyProtocol>
{
    public ExamDutyProtocolsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<ExamDutyProtocol>, IQueryable<ExamDutyProtocol>>[] Includes =>
        new Func<IQueryable<ExamDutyProtocol>, IQueryable<ExamDutyProtocol>>[]
        {
            (q) => q.Include(e => e.Classes),
            (q) => q.Include(e => e.Students),
            (q) => q.Include(e => e.Supervisors)
        };
}
