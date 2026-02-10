namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateParentMeetingCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ParentMeeting> ParentMeetingAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateParentMeetingCommand, int>
{
    public async Task<int> Handle(UpdateParentMeetingCommand command, CancellationToken ct)
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

        parentMeeting.UpdateData(
            command.Date!.Value,
            TimeSpan.Parse(command.StartTime!),
            command.Location,
            command.Title!,
            command.Description,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);

        return parentMeeting.ParentMeetingId;
    }
}
