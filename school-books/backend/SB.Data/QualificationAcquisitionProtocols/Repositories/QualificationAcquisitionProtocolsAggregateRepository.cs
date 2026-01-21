namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;

internal class QualificationAcquisitionProtocolsAggregateRepository : ScopedAggregateRepository<QualificationAcquisitionProtocol>
{
    public QualificationAcquisitionProtocolsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<QualificationAcquisitionProtocol>, IQueryable<QualificationAcquisitionProtocol>>[] Includes =>
        new Func<IQueryable<QualificationAcquisitionProtocol>, IQueryable<QualificationAcquisitionProtocol>>[]
        {
            (q) => q.Include(e => e.Students),
            (q) => q.Include(e => e.Commissioners)
        };
}
