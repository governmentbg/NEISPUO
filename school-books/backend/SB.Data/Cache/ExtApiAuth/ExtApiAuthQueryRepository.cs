namespace SB.Data;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Domain;
using static SB.Domain.IExtApiAuthQueryRepository;

internal class ExtApiAuthQueryRepository : Repository, IExtApiAuthQueryRepository
{
    public ExtApiAuthQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetExtSystemVO?> GetExtSystemAsync(string thumbprint, CancellationToken ct)
    {
        var res = await (
            from s in this.DbContext.Set<ExtSystem>()
            join c in this.DbContext.Set<ExtSystemCertificate>() on s.ExtSystemId equals c.ExtSystemId
            join a in this.DbContext.Set<ExtSystemAccess>() on s.ExtSystemId equals a.ExtSystemId

            where c.Thumbprint == thumbprint &&
                c.IsValid &&
                s.IsValid &&
                a.IsValid

            select new
            {
                s.ExtSystemId,
                s.SysUserId,
                a.ExtSystemType
            }
        ).ToArrayAsync(ct);

        return res.GroupBy(r => new { r.ExtSystemId, r.SysUserId })
            .Select(g => new GetExtSystemVO(
                g.Key.ExtSystemId,
                g.Select(r => r.ExtSystemType).ToArray(),
                g.Key.SysUserId))
            .SingleOrDefault();
    }
}
