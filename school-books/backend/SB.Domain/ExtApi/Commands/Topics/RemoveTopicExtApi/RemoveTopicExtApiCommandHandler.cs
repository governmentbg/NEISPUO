namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveTopicExtApiCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Topic> TopicAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<RemoveTopicExtApiCommand>
{
    public async Task Handle(RemoveTopicExtApiCommand command, CancellationToken ct)
    {
        var topic = await this.TopicAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.TopicId!.Value,
            ct);

        if (command.ClassBookId!.Value != topic.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        if (!await this.ClassBookCachedQueryStore.CheckClassBookIsValidAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                ct))
        {
            throw new DomainValidationException($"The classbook is marked as invalid (archived).");
        }

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsAttendanceAbsenceTopicModificationsAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            topic.Date,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        this.TopicAggregateRepository.Remove(topic);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
