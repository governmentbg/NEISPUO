namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record RemoveTeacherAbsenceCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<TeacherAbsence> TeacherAbsenceAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<RemoveTeacherAbsenceCommand>
{
    public async Task Handle(RemoveTeacherAbsenceCommand command, CancellationToken ct)
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

        this.TeacherAbsenceAggregateRepository.Remove(teacherAbsence);
        await this.UnitOfWork.SaveAsync(ct);

        var relatedScheduleLessonIds = teacherAbsence.Hours.Select(h => h.ScheduleLessonId);

        foreach (var scheduleLessonId in relatedScheduleLessonIds)
        {
            await this.ClassBookCachedQueryStore.ClearScheduleLessonTeacherAbsenceIdAsync(
                command.SchoolYear!.Value,
                scheduleLessonId,
                // no cancellation
                default);
        }
    }
}
