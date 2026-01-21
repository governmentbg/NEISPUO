namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;

internal class QualificationExamResultProtocolsAggregateRepository : ScopedAggregateRepository<QualificationExamResultProtocol>
{
    public QualificationExamResultProtocolsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<QualificationExamResultProtocol>, IQueryable<QualificationExamResultProtocol>>[] Includes =>
        new Func<IQueryable<QualificationExamResultProtocol>, IQueryable<QualificationExamResultProtocol>>[]
        {
            (q) => q.Include(e => e.Classes),
            (q) => q.Include(e => e.Students),
            (q) => q.Include(e => e.Commissioners)
        };
}
