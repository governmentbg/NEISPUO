namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record RemoveAbsenceCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Absence> AbsenceAggregateRepository,
    IQueueMessagesService QueueMessagesService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<RemoveAbsenceCommand>
{
    public async Task Handle(RemoveAbsenceCommand command, CancellationToken ct)
    {
        var absence = await this.AbsenceAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.AbsenceId!.Value,
            ct);

        if (command.ClassBookId!.Value != absence.ClassBookId)
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
            absence.Date,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        this.AbsenceAggregateRepository.Remove(absence);

        await this.QueueMessagesService.CancelMessagesAndSaveAsync<NotificationQueueMessage>(absence.EmailTag, ct);
        await this.QueueMessagesService.CancelMessagesAndSaveAsync<NotificationQueueMessage>(absence.PushNotificationTag, ct);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
