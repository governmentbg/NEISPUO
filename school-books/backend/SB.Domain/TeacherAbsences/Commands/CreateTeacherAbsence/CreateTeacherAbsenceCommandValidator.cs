namespace SB.Domain;

using System.Linq;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SB.Common;

public class CreateTeacherAbsenceCommandValidator : AbstractValidator<CreateTeacherAbsenceCommand>
{
    public CreateTeacherAbsenceCommandValidator(IValidator<CreateTeacherAbsenceCommandHour> hourValidator)
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.TeacherPersonId).NotEmpty();
        this.RuleFor(c => c.StartDate).NotEmpty();
        this.RuleFor(c => c.EndDate).NotEmpty();
        this.RuleFor(c => c.Reason).NotEmpty().MaximumLength(1000);
        this.RuleFor(c => c.Hours).NotEmpty();
        this.RuleForEach(c => c.Hours).SetValidator(hourValidator);

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                if (c.Hours!.Where(h => h.ReplTeacherPersonId == c.TeacherPersonId!.Value).Any())
                {
                    context.AddUserFailure("Не може да се създаде учителско отсъствие в което учителя замества сам себе си.");
                }

                var teacherAbsencesQueryRepository = context.GetServiceProvider().GetRequiredService<ITeacherAbsencesQueryRepository>();

                var lessons = await teacherAbsencesQueryRepository.GetLessonsAsync(
                    c.SchoolYear!.Value,
                    c.InstId!.Value,
                    c.Hours!.Select(h => h.ScheduleLessonId!.Value).ToArray(),
                    ct);

                var submittedScheduleLessonIds = c.Hours!.Select(h => h.ScheduleLessonId!.Value).ToHashSet();
                var actualScheduleLessonIds = lessons.Select(l =>l.ScheduleLessonId).ToHashSet();
                if (!submittedScheduleLessonIds.SetEquals(actualScheduleLessonIds))
                {
                    submittedScheduleLessonIds.ExceptWith(actualScheduleLessonIds);

                    var scheduleLessonIdsString = string.Join(", ", submittedScheduleLessonIds.Select(id => id.ToString()));

                    context.AddUnexpectedFailure($"This scheduleLesson ({scheduleLessonIdsString}) is not in any of the institution's schedules.");
                }

                if (lessons.Length != c.Hours!.Length)
                {
                    context.AddUnexpectedFailure("Schedule lesson is outside start/end date");
                    return;
                }

                foreach (var lesson in lessons)
                {
                    if (lesson.Date < c.StartDate!.Value || lesson.Date > c.EndDate!.Value)
                    {
                        context.AddUnexpectedFailure("Schedule lesson is outside start/end date");
                        return;
                    }

                    if (!lesson.CurriculumTeacherPersonIds.Contains(c.TeacherPersonId!.Value) &&
                        !(c.TeacherAbsenceId != null && c.TeacherAbsenceId == lesson.TeacherAbsenceId))
                    {
                        context.AddUnexpectedFailure("Schedule lesson is not for selected teacher");
                        return;
                    }
                }

                var lessonsInUse = await teacherAbsencesQueryRepository.GetLessonsInUseAsync(
                    c.SchoolYear!.Value,
                    c.InstId!.Value,
                    c.Hours!.Select(h => h.ScheduleLessonId!.Value).ToArray(),
                    c.TeacherAbsenceId,
                    ct);

                if (lessonsInUse.Any())
                {
                    var hoursString = string.Join(
                        ", ",
                        lessonsInUse.Select(h => $"{h.ClassName}/{h.Date:dd.MM.yyyy}/#{h.HourNumber}"));
                    context.AddUserFailure(
                        $"Не може да се създаде учителско отсъствие за часове, за които има въведени оценки/отсътвия/теми/учителски отсъствия - {hoursString.TruncateWithEllipsis(100)}.");
                }

                var actualAbsenceScheduleLessonIds =
                    lessons
                        .Where(l => l.TeacherAbsenceId == c.TeacherAbsenceId)
                        .Select(l => l.ScheduleLessonId)
                        .ToHashSet();
                var newScheduleLessonIds = submittedScheduleLessonIds.Except(actualAbsenceScheduleLessonIds).ToHashSet();

                var classBooks =
                    lessons
                        .Where(l => newScheduleLessonIds.Contains(l.ScheduleLessonId))
                        .Select(l => l.ClassBookId)
                        .ToHashSet();

                var offDayDates = await teacherAbsencesQueryRepository.GetOffDayDatesForClassBooksAsync(
                    c.SchoolYear!.Value,
                    c.InstId!.Value,
                    c.StartDate!.Value,
                    c.EndDate!.Value,
                    classBooks.ToArray(),
                    ct);

                if (offDayDates.Any())
                {
                    var lessonDates = lessons.Select(l => l.Date).ToHashSet();
                    var lessonClassBookDates = lessons
                        .Select(l => (l.ClassBookId, l.Date))
                        .ToHashSet();

                    var overlappingDates = offDayDates
                        .Where(odd => lessonClassBookDates.Contains((odd.ClassBookId, odd.Date)))
                        .ToHashSet();

                    if (overlappingDates.Any())
                    {
                        context.AddUnexpectedFailure("Cannot create teacher absence for days which are marked as off day");
                    }
                }
            });
    }
}
