namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record RemoveGradeCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Grade> GradeAggregateRepository,
    IQueueMessagesService QueueMessagesService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<RemoveGradeCommand>
{
    public async Task Handle(RemoveGradeCommand command, CancellationToken ct)
    {
        var grade = await this.GradeAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.GradeId!.Value,
            ct);

        if (command.ClassBookId!.Value != grade.ClassBookId)
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

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsGradeModificationsAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            grade.Type,
            grade.Date,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        this.GradeAggregateRepository.Remove(grade);

        await this.QueueMessagesService.CancelMessagesAndSaveAsync<NotificationQueueMessage>(grade.EmailTag, ct);
        await this.QueueMessagesService.CancelMessagesAndSaveAsync<NotificationQueueMessage>(grade.PushNotificationTag, ct);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
