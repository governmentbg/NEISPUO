namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record ConvertToLateAbsenceCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Absence> AbsenceAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<ConvertToLateAbsenceCommand>
{
    public async Task Handle(ConvertToLateAbsenceCommand command, CancellationToken ct)
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

        absence.ConvertToLate(command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
