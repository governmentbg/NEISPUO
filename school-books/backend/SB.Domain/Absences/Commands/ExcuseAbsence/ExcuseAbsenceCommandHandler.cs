namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record ExcuseAbsenceCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Absence> AbsenceAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<ExcuseAbsenceCommand>
{
    public async Task Handle(ExcuseAbsenceCommand command, CancellationToken ct)
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

        absence.Excuse(command.ExcusedReasonId, command.ExcusedReasonComment, command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
