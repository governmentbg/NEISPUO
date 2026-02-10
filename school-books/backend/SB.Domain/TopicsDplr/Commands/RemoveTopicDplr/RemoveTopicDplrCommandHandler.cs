namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record RemoveTopicDplrCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<TopicDplr> TopicDplrAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<RemoveTopicDplrCommand>
{
    public async Task Handle(RemoveTopicDplrCommand command, CancellationToken ct)
    {
        var topic = await this.TopicDplrAggregateRepository.FindAsync(
                 command.SchoolYear!.Value,
                 command.TopicDplrId!.Value,
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

        this.TopicDplrAggregateRepository.Remove(topic);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
