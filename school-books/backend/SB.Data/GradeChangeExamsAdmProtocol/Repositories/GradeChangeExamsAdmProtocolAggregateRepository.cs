namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;

internal class GradeChangeExamsAdmProtocolAggregateRepository : ScopedAggregateRepository<GradeChangeExamsAdmProtocol>
{
    public GradeChangeExamsAdmProtocolAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<GradeChangeExamsAdmProtocol>, IQueryable<GradeChangeExamsAdmProtocol>>[] Includes =>
        new Func<IQueryable<GradeChangeExamsAdmProtocol>, IQueryable<GradeChangeExamsAdmProtocol>>[]
        {
            (q) => q.Include(e => e.Commissioners),
            (q) => q.Include(e => e.Students).ThenInclude(e => e.Subjects)
        };
}
