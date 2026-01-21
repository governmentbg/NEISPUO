namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class TopicsAggregateRepository : ScopedAggregateRepository<Topic>, ITopicsAggregateRepository
{
    public TopicsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<Topic>, IQueryable<Topic>>[] Includes =>
        new Func<IQueryable<Topic>, IQueryable<Topic>>[]
        {
            (q) => q.Include(t => t.Titles),
            (q) => q.Include(t => t.Teachers),
        };

    public async Task<Topic[]> FindUsedTopicsAsync(
        int schoolYear,
        int classBookId,
        int[] classBookTopicPlanItemIds,
        CancellationToken ct)
    {
        return await this.FindEntitiesAsync(

            t => t.SchoolYear == schoolYear &&
                t.ClassBookId == classBookId &&
                t.Titles.Any(tt =>
                    this.DbContext.MakeIdsQuery(classBookTopicPlanItemIds)
                    .Any(id => tt.ClassBookTopicPlanItemId == id.Id)),
            ct);
    }
}
