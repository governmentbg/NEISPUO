namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateAbsenceExtApiCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Absence> AbsenceAggregateRepository,
    INotificationsQueryRepository NotificationsQueryRepository,
    INotificationsService NotificationsService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore
    )
    : IRequestHandler<CreateAbsenceExtApiCommand, int>
{
    public async Task<int> Handle(CreateAbsenceExtApiCommand command, CancellationToken ct)
    {
        int schoolYear = command.SchoolYear!.Value;
        int instId = command.InstId!.Value;
        int classBookId = command.ClassBookId!.Value;
        int sysUserId = command.SysUserId!.Value;

        if (!await this.ClassBookCachedQueryStore.ExistsClassBookAsync(
            schoolYear,
            classBookId,
            ct))
        {
            throw new DomainValidationException($"A classbook with (schoolYear:{schoolYear}, classBookId:{classBookId}) does not exist.");
        }

        if (!await this.ClassBookCachedQueryStore.CheckClassBookIsValidAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                ct))
        {
            throw new DomainValidationException($"The classbook is marked as invalid (archived).");
        }

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsAttendanceAbsenceTopicModificationsAsync(
            schoolYear,
            classBookId,
            command.Date!.Value,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        if (!await this.ClassBookCachedQueryStore.CheckBookTypeAllowsAbsenceTypeAsync(
                schoolYear,
                classBookId,
                command.Type!.Value,
                ct))
        {
            throw new DomainValidationException($"Cannot create absence for the book type of classBookId:{classBookId}");
        }

        if (!await this.ClassBookCachedQueryStore.ExistsStudentForClassBookAsync(
                schoolYear,
                classBookId,
                command.PersonId!.Value,
                ct))
        {
            throw new DomainValidationException("This person is not in the class book students list");
        }

        if (!await this.ClassBookCachedQueryStore.ExistsScheduleLessonForClassBookAsync(
                schoolYear,
                classBookId,
                command.Date!.Value,
                command.ScheduleLessonId!.Value,
                command.PersonId!.Value,
                ct))
        {
            throw new DomainValidationException("This scheduleLesson is not in any of the classbook's schedules.");
        }

        var teacherAbsenceId = await this.ClassBookCachedQueryStore.GetScheduleLessonTeacherAbsenceIdAsync(
            schoolYear,
            command.ScheduleLessonId!.Value,
            ct);

        Absence absence = new(
            schoolYear,
            classBookId,
            command.PersonId!.Value,
            command.Type!.Value,
            await this.ClassBookCachedQueryStore.GetTermForDateAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                command.Date!.Value, ct),
            command.Date!.Value,
            command.ScheduleLessonId!.Value,
            teacherAbsenceId,
            command.ExcusedReason,
            command.ExcusedReasonComment,
            command.SysUserId!.Value);

        await this.AbsenceAggregateRepository.AddAsync(absence, ct);

        await this.UnitOfWork.SaveAsync(ct);

        return absence.AbsenceId;
    }
}
