namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateTopicPlanItemCommandHandler(
    IUnitOfWork UnitOfWork,
    IAggregateRepository<TopicPlan> TopicPlanAggregateRepository,
    IAggregateRepository<TopicPlanItem> TopicPlanItemAggregateRepository)
    : IRequestHandler<CreateTopicPlanItemCommand, int>
{
    public async Task<int> Handle(CreateTopicPlanItemCommand command, CancellationToken ct)
    {
        var topicPlan = await this.TopicPlanAggregateRepository.FindAsync(
            command.TopicPlanId!.Value,
            ct);

        if (topicPlan.CreatedBySysUserId != command.SysUserId!.Value)
        {
            throw new DomainValidationException("This topic is created by another user");
        }

        var topicPlanItem = new TopicPlanItem(
            command.TopicPlanId!.Value,
            command.Number!.Value,
            command.Title!,
            command.Note);

        await this.TopicPlanItemAggregateRepository.AddAsync(topicPlanItem, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return topicPlanItem.TopicPlanItemId;
    }
}
