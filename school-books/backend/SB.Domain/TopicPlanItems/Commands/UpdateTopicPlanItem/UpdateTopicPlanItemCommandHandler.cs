namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record UpdateTopicPlanItemCommandHandler(
    IUnitOfWork UnitOfWork,
    IAggregateRepository<TopicPlan> TopicPlanAggregateRepository,
    IAggregateRepository<TopicPlanItem> TopicPlanItemAggregateRepository)
    : IRequestHandler<UpdateTopicPlanItemCommand, int>
{
    public async Task<int> Handle(UpdateTopicPlanItemCommand command, CancellationToken ct)
    {
        var topicPlanItem = await this.TopicPlanItemAggregateRepository.FindAsync(
            command.TopicPlanItemId!.Value,
            ct);

        var topicPlan = await this.TopicPlanAggregateRepository.FindAsync(
            topicPlanItem.TopicPlanId,
            ct);

        if (topicPlan.CreatedBySysUserId != command.SysUserId!.Value)
        {
            throw new DomainValidationException("This topic is created by another user");
        }

        topicPlanItem.Update(
            command.Number!.Value,
            command.Title!,
            command.Note);
        await this.UnitOfWork.SaveAsync(ct);

        return topicPlanItem.TopicPlanItemId;
    }
}
