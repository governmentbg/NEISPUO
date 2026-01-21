namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateTopicPlanCommandHandler(
    IUnitOfWork UnitOfWork,
    IAggregateRepository<TopicPlan> TopicPlanAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateTopicPlanCommand, int>
{
    public async Task<int> Handle(UpdateTopicPlanCommand command, CancellationToken ct)
    {
        var topicPlan = await this.TopicPlanAggregateRepository.FindAsync(
            command.TopicPlanId!.Value,
            ct);

        if (topicPlan.CreatedBySysUserId != command.SysUserId!.Value)
        {
            throw new DomainValidationException("This topic is created by another user");
        }

        topicPlan.Update(
            command.Name!,
            command.BasicClassId,
            command.SubjectId,
            command.SubjectTypeId,
            command.TopicPlanPublisherId,
            command.TopicPlanPublisherOther);
        await this.UnitOfWork.SaveAsync(ct);

        return topicPlan.TopicPlanId;
    }
}
