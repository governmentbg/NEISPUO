namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateParentMeetingCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ParentMeeting> ParentMeetingAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateParentMeetingCommand, int>
{
    public async Task<int> Handle(CreateParentMeetingCommand command, CancellationToken ct)
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

        var parentMeeting = new ParentMeeting(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.Date!.Value,
            TimeSpan.Parse(command.StartTime!),
            command.Location,
            command.Title!,
            command.Description,
            command.SysUserId!.Value);

        await this.ParentMeetingAggregateRepository.AddAsync(parentMeeting, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return parentMeeting.ParentMeetingId;
    }
}
