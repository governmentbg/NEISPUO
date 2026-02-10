namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record ExcuseAttendanceCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Attendance> AttendanceAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<ExcuseAttendanceCommand>
{
    public async Task Handle(ExcuseAttendanceCommand command, CancellationToken ct)
    {
        var attendance = await this.AttendanceAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.AttendanceId!.Value,
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

        attendance.Excuse(command.ExcusedReasonId, command.ExcusedReasonComment, command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
