namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateAttendanceExtApiCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Attendance> AttendanceAggregateRepository,
    IAttendancesQueryRepository AttendancesQueryRepository,
    INotificationsQueryRepository NotificationsQueryRepository,
    INotificationsService NotificationsService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateAttendanceExtApiCommand, int>
{
    public async Task<int> Handle(CreateAttendanceExtApiCommand command, CancellationToken ct)
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

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsAttendanceAbsenceTopicModificationsAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.Date!.Value,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        int schoolYear = command.SchoolYear!.Value;
        int classBookId = command.ClassBookId!.Value;

        var schoolYearLimits = await this.AttendancesQueryRepository
            .GetSchoolYearLimitsAsync(schoolYear, classBookId, ct);

        if (command.Date!.Value < schoolYearLimits.SchoolYearStartDateLimit ||
            command.Date!.Value > schoolYearLimits.SchoolYearEndDateLimit)
        {
            throw new DomainValidationException("Cannot create attendance outside of school year limits");
        }

        if (!await this.ClassBookCachedQueryStore.CheckBookTypeAllowsAttendancesAsync(schoolYear, classBookId, ct))
        {
            throw new DomainValidationException($"Cannot create attendance for the book type of classBookId:{classBookId}");
        }

        if (!await this.ClassBookCachedQueryStore.ExistsStudentForClassBookAsync(
                schoolYear,
                classBookId,
                command.PersonId!.Value,
                ct))
        {
            throw new DomainValidationException("This person is not in the class book students list");
        }

        var attendance = new Attendance(
            schoolYear,
            classBookId,
            command.PersonId!.Value,
            command.Type!.Value,
            command.Date!.Value,
            command.ExcusedReasonId,
            command.ExcusedReasonComment,
            command.SysUserId!.Value);

        await this.AttendanceAggregateRepository.AddAsync(attendance, ct);


        await this.UnitOfWork.SaveAsync(ct);

        return attendance.AttendanceId;
    }
}
