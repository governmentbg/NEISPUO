namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;

internal class GraduationThesisDefenseProtocolAggregateRepository : ScopedAggregateRepository<GraduationThesisDefenseProtocol>
{
    public GraduationThesisDefenseProtocolAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<GraduationThesisDefenseProtocol>, IQueryable<GraduationThesisDefenseProtocol>>[] Includes =>
        new Func<IQueryable<GraduationThesisDefenseProtocol>, IQueryable<GraduationThesisDefenseProtocol>>[]
        {
            (q) => q.Include(e => e.Commissioners)
        };
}
