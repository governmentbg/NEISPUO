namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;

internal class ExamResultProtocolsAggregateRepository : ScopedAggregateRepository<ExamResultProtocol>
{
    public ExamResultProtocolsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<ExamResultProtocol>, IQueryable<ExamResultProtocol>>[] Includes =>
        new Func<IQueryable<ExamResultProtocol>, IQueryable<ExamResultProtocol>>[]
        {
            (q) => q.Include(e => e.Classes),
            (q) => q.Include(e => e.Students),
            (q) => q.Include(e => e.Commissioners)
        };
}
