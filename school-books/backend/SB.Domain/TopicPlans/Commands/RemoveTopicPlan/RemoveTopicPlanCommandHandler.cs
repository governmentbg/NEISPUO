namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveTopicPlanCommandHandler(
    IUnitOfWork UnitOfWork,
    IAggregateRepository<TopicPlan> TopicPlanAggregateRepository,
    ITopicPlansQueryRepository TopicPlansQueryRepository)
    : IRequestHandler<RemoveTopicPlanCommand>
{
    public async Task Handle(RemoveTopicPlanCommand command, CancellationToken ct)
    {
        var topicPlan = await this.TopicPlanAggregateRepository.FindAsync(
            command.TopicPlanId!.Value,
            ct);

        if (topicPlan.CreatedBySysUserId != command.SysUserId!.Value)
        {
            throw new DomainValidationException("This topic is created by another user");
        }

        await this.TopicPlansQueryRepository.RemoveTopicPlanAsync(command.TopicPlanId!.Value, ct);
    }
}
