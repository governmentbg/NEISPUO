namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveAllTopicPlanItemsCommandHandler(
    IUnitOfWork UnitOfWork,
    IAggregateRepository<TopicPlan> TopicPlanAggregateRepository,
    ITopicPlanItemsAggregateRepository TopicPlanItemsAggregateRepository)
    : IRequestHandler<RemoveAllTopicPlanItemsCommand>
{
    public async Task Handle(RemoveAllTopicPlanItemsCommand command, CancellationToken ct)
    {
        var topicPlan = await this.TopicPlanAggregateRepository.FindAsync(
            command.TopicPlanId!.Value,
            ct);

        if (topicPlan.CreatedBySysUserId != command.SysUserId!.Value)
        {
            throw new DomainValidationException("This topic plan is created by another user");
        }

        var topicPlanItems = await this.TopicPlanItemsAggregateRepository.FindAllByTopicPlanIdAsync(
            command.TopicPlanId!.Value,
            ct);

        foreach (var topicPlanItem in topicPlanItems)
        {
            this.TopicPlanItemsAggregateRepository.Remove(topicPlanItem);
        }

        await this.UnitOfWork.SaveAsync(ct);
    }
}
