namespace SB.Domain;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record RemoveParentMeetingCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ParentMeeting> ParentMeetingAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<RemoveParentMeetingCommand>
{
    public async Task Handle(RemoveParentMeetingCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.ExistsClassBookAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct))
        {
            throw new DomainValidationException($"A classbook with (schoolYear:{command.SchoolYear!.Value}, classBookId:{command.ClassBookId!.Value}) does not exist.");
        }

        if (!await this.ClassBookCachedQueryStore.CheckClassBookIsValidAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                ct))
        {
            throw new DomainValidationException($"The classbook is marked as invalid (archived).");
        }

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        var parentMeeting = await this.ParentMeetingAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ParentMeetingId!.Value,
            ct);

        if (command.ClassBookId!.Value != parentMeeting.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        this.ParentMeetingAggregateRepository.Remove(parentMeeting);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
