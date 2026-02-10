namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record RemoveTopicsCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Topic> TopicAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<RemoveTopicsCommand>
{
    public async Task Handle(RemoveTopicsCommand command, CancellationToken ct)
    {
        foreach (var commandTopic in command.Topics!)
        {
            var topic = await this.TopicAggregateRepository.FindAsync(
                command.SchoolYear!.Value,
                commandTopic.TopicId!.Value,
                ct);

            if (command.ClassBookId!.Value != topic.ClassBookId)
            {
                // the classBookId check is required as it is part of the auth checks
                throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
            }

            if (commandTopic.ScheduleLessonId!.Value != topic.ScheduleLessonId)
            {
                // the schedule lesson check is required as it is part of the auth checks
                throw new DomainValidationException($"Incorrect {nameof(commandTopic.ScheduleLessonId)}.");
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
        }

        await this.UnitOfWork.SaveAsync(ct);
    }
}
