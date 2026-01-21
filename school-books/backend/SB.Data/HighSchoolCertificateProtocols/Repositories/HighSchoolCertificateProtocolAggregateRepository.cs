namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;

internal class HighSchoolCertificateProtocolAggregateRepository : ScopedAggregateRepository<HighSchoolCertificateProtocol>
{
    public HighSchoolCertificateProtocolAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<HighSchoolCertificateProtocol>, IQueryable<HighSchoolCertificateProtocol>>[] Includes =>
        new Func<IQueryable<HighSchoolCertificateProtocol>, IQueryable<HighSchoolCertificateProtocol>>[]
        {
            (q) => q.Include(e => e.Commissioners),
            (q) => q.Include(e => e.Students)
        };
}
