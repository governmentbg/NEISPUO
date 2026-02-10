namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateTeacherAbsenceCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<TeacherAbsence> TeacherAbsenceAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateTeacherAbsenceCommand, int>
{
    public async Task<int> Handle(UpdateTeacherAbsenceCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.CheckSchoolYearAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            ct))
        {
            throw new DomainValidationException($"The school year is locked.");
        }

        var teacherAbsence = await this.TeacherAbsenceAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.TeacherAbsenceId!.Value,
            ct);

        if (command.InstId!.Value != teacherAbsence.InstId)
        {
            // the instId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.InstId)}.");
        }

        // TeacherPersonId, StartDate and EndDate are passed in the command
        // only as a perf optimization as they are needed in the validator
        // and should not be different than the ones contained in the teacher absence
        if (teacherAbsence.TeacherPersonId != command.TeacherPersonId!.Value ||
            teacherAbsence.StartDate != command.StartDate!.Value ||
            teacherAbsence.EndDate != command.EndDate!.Value)
        {
            throw new DomainValidationException("Cannot change the TeacherPersonId, StartDate or EndDate!");
        }

        var scheduleLessonIdsForRemoval = teacherAbsence.Hours.Select(h => h.ScheduleLessonId).Except(command.Hours!.Select(h => h.ScheduleLessonId!.Value));
        foreach (var scheduleLessonId in scheduleLessonIdsForRemoval)
        {
            await this.ClassBookCachedQueryStore.ClearScheduleLessonTeacherAbsenceIdAsync(
                command.SchoolYear!.Value,
                scheduleLessonId,
                // no cancellation
                default);
        }

        teacherAbsence.UpdateData(
            command.Reason!,
            command.Hours!.Select(h => (
                scheduleLessonId: h.ScheduleLessonId!.Value,
                replTeacherPersonId: h.ReplTeacherPersonId,
                replTeacherIsNonSpecialist: h.ReplTeacherIsNonSpecialist,
                extReplTeacherName: h.ExtReplTeacherName
            )).ToArray(),
            command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);

        return teacherAbsence.TeacherAbsenceId;
    }
}
