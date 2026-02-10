namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IReplrParticipationsQueryRepository;

internal class ReplrParticipationsQueryRepository : Repository, IReplrParticipationsQueryRepository
{
    public ReplrParticipationsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int classBookId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await(
            from p in this.DbContext.Set<ReplrParticipation>()

            where p.SchoolYear == schoolYear &&
                p.ClassBookId == classBookId

            orderby p.Date

            select new GetAllVO(
                p.ReplrParticipationId,
                p.Date,
                p.Topic))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        int replrParticipationId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<ReplrParticipation>()
            .Where(s =>
                s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId &&
                s.ReplrParticipationId == replrParticipationId)
            .Select(s => new GetVO(
                s.ReplrParticipationId,
                s.ReplrParticipationTypeId,
                s.Date,
                s.Topic,
                s.InstId,
                s.Attendees))
            .SingleAsync(ct);
    }
}
