namespace SB.Domain;

using System.Linq;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SB.Common;

public class CreateLectureScheduleCommandValidator : AbstractValidator<CreateLectureScheduleCommand>
{
    public CreateLectureScheduleCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.TeacherPersonId).NotNull();
        this.RuleFor(c => c.OrderNumber).NotEmpty();
        this.RuleFor(c => c.OrderDate).NotNull();
        this.RuleFor(c => c.StartDate).NotNull();
        this.RuleFor(c => c.EndDate).NotNull();
        this.RuleFor(c => c.ScheduleLessonIds).NotEmpty();

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var lectureSchedulesQueryRepository = context.GetServiceProvider().GetRequiredService<ILectureSchedulesQueryRepository>();

                var lessons = await lectureSchedulesQueryRepository.GetLessonsAsync(
                    c.SchoolYear!.Value,
                    c.InstId!.Value,
                    c.ScheduleLessonIds!,
                    ct);

                foreach (var lesson in lessons)
                {
                    if (lesson.Date < c.StartDate!.Value || lesson.Date > c.EndDate!.Value)
                    {
                        context.AddUnexpectedFailure("Schedule lesson is outside start/end date");
                        return;
                    }

                    if (!lesson.CurriculumTeacherPersonIds.Contains(c.TeacherPersonId!.Value) &&
                        !(c.LectureScheduleId != null && c.LectureScheduleId == lesson.LectureScheduleId))
                    {
                        context.AddUnexpectedFailure("Schedule lesson is not for selected teacher");
                        return;
                    }
                }

                var lectureScheduleHoursInUse = await lectureSchedulesQueryRepository.GetLessonsInUseAsync(
                    c.SchoolYear!.Value,
                    c.InstId!.Value,
                    c.ScheduleLessonIds!,
                    c.LectureScheduleId,
                    ct);

                if (lectureScheduleHoursInUse.Any())
                {
                    var hoursString = string.Join(", ", lectureScheduleHoursInUse.Select(h => $"{h.ClassName}/{h.Date:dd.MM.yyyy}/#{h.HourNumber}"));

                    context.AddUserFailure(
                        $"Не може да се създаде лекторски график за часове, които участват в друг лекторски график - {hoursString.TruncateWithEllipsis(100)}.");
                }
            });
    }
}
