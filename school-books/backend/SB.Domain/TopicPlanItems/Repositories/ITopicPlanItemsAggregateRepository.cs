namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public interface ITopicPlanItemsAggregateRepository : IScopedAggregateRepository<TopicPlanItem>
{
    Task<TopicPlanItem[]> FindAllByTopicPlanIdAsync(
        int topicPlanId,
        CancellationToken ct);
}
