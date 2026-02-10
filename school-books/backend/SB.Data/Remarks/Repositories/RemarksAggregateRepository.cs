namespace SB.Data;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SB.Common;
using SB.Domain;

internal class RemarksAggregateRepository : ScopedAggregateRepository<Remark>, IRemarksAggregateRepository
{
    public RemarksAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<Remark[]> FindAllByIdsAsync(
        int schoolYear,
        int[] remarkIds,
        CancellationToken ct)
    {
        return await this.FindEntitiesAsync(
            a => a.SchoolYear == schoolYear && this.DbContext.MakeIdsQuery(remarkIds).Any(id => a.RemarkId == id.Id),
            ct);
    }
}
