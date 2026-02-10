namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IParentMeetingsQueryRepository;

internal class ParentMeetingsQueryRepository : Repository, IParentMeetingsQueryRepository
{
    public ParentMeetingsQueryRepository(UnitOfWork unitOfWork)
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
        return await this.DbContext.Set<ParentMeeting>()
            .Where(pm =>
                pm.SchoolYear == schoolYear &&
                pm.ClassBookId == classBookId)
            .OrderByDescending(pm => pm.Date)
            .Select(pm => new GetAllVO(
                pm.ParentMeetingId,
                pm.Date,
                pm.StartTime,
                pm.Location,
                pm.Title))
            .ToTableResultAsync(offset ?? 0, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        int parentMeetingId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<ParentMeeting>()
            .Where(pm =>
                pm.SchoolYear == schoolYear &&
                pm.ClassBookId == classBookId &&
                pm.ParentMeetingId == parentMeetingId)
            .Select(pm => new GetVO(
                pm.ParentMeetingId,
                pm.Date,
                pm.StartTime,
                pm.Location,
                pm.Title,
                pm.Description
            ))
            .SingleAsync(ct);
    }
}
