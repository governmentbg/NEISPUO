namespace SB.Data;

using SB.Domain;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

internal class TopicPlanItemsAggregateRepository : ScopedAggregateRepository<TopicPlanItem>, ITopicPlanItemsAggregateRepository
{
    public TopicPlanItemsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TopicPlanItem[]> FindAllByTopicPlanIdAsync(
        int topicPlanId,
        CancellationToken ct)
    {
        return (await this.FindEntitiesAsync(
            tpi => tpi.TopicPlanId == topicPlanId,
            ct)
        ).ToArray();
    }
}
