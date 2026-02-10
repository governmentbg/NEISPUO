namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveTopicPlanItemCommandHandler(
    IUnitOfWork UnitOfWork,
    IAggregateRepository<TopicPlan> TopicPlanAggregateRepository,
    IAggregateRepository<TopicPlanItem> TopicPlanItemAggregateRepository)
    : IRequestHandler<RemoveTopicPlanItemCommand>
{
    public async Task Handle(RemoveTopicPlanItemCommand command, CancellationToken ct)
    {
        var topicPlanItem = await this.TopicPlanItemAggregateRepository.FindAsync(
            command.TopicPlanItemId!.Value,
            ct);

        var topicPlan = await this.TopicPlanAggregateRepository.FindAsync(
            topicPlanItem.TopicPlanId,
            ct);

        if (topicPlan.CreatedBySysUserId != command.SysUserId!.Value)
        {
            throw new DomainValidationException("This topic plan is created by another user");
        }

        this.TopicPlanItemAggregateRepository.Remove(topicPlanItem);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
