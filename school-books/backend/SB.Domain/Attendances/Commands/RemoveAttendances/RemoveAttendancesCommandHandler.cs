namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record RemoveAttendancesCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Attendance> AttendanceAggregateRepository,
    IQueueMessagesService QueueMessagesService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<RemoveAttendancesCommand>
{
    public async Task Handle(RemoveAttendancesCommand command, CancellationToken ct)
    {
        foreach (var attendanceId in command.AttendanceIds!)
        {
            var attendance = await this.AttendanceAggregateRepository.FindAsync(
                command.SchoolYear!.Value,
                attendanceId,
                ct);

            if (command.ClassBookId!.Value != attendance.ClassBookId)
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
                attendance.Date,
                ct))
            {
                throw new DomainValidationException($"The classbook is locked.");
            }

            this.AttendanceAggregateRepository.Remove(attendance);
            await this.QueueMessagesService.CancelMessagesAndSaveAsync<NotificationQueueMessage>(attendance.EmailTag, ct);
            await this.QueueMessagesService.CancelMessagesAndSaveAsync<NotificationQueueMessage>(attendance.PushNotificationTag, ct);
        }

        await this.UnitOfWork.SaveAsync(ct);
    }
}
