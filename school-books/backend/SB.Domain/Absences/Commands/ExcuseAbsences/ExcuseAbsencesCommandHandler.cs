namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record ExcuseAbsencesCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Absence> AbsenceAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<ExcuseAbsencesCommand>
{
    public async Task Handle(ExcuseAbsencesCommand command, CancellationToken ct)
    {
        foreach (var commandAbsence in command.Absences!)
        {
            var absence = await this.AbsenceAggregateRepository.FindAsync(
                command.SchoolYear!.Value,
                commandAbsence.AbsenceId!.Value,
                ct);

            if (command.ClassBookId!.Value != absence.ClassBookId)
            {
                // the classBookId check is required as it is part of the auth checks
                throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
            }

            if (commandAbsence.ScheduleLessonId!.Value != absence.ScheduleLessonId)
            {
                // the schedule lesson check is required as it is part of the auth checks
                throw new DomainValidationException($"Incorrect {nameof(commandAbsence.ScheduleLessonId)}.");
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
        }

        await this.UnitOfWork.SaveAsync(ct);
    }
}
