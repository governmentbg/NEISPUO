namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateTopicPlanCommandHandler(
    IUnitOfWork UnitOfWork,
    IAggregateRepository<TopicPlan> TopicPlanAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateTopicPlanCommand, int>
{
    public async Task<int> Handle(CreateTopicPlanCommand command, CancellationToken ct)
    {
        var topicPlan = new TopicPlan(
            command.SysUserId!.Value,
            command.Name!,
            command.BasicClassId,
            command.SubjectId,
            command.SubjectTypeId,
            command.TopicPlanPublisherId,
            command.TopicPlanPublisherOther);

        await this.TopicPlanAggregateRepository.AddAsync(topicPlan, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return topicPlan.TopicPlanId;
    }
}
