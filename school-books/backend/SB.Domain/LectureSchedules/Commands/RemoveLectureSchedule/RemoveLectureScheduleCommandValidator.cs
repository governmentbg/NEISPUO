namespace SB.Domain;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public class RemoveLectureScheduleCommandValidator : AbstractValidator<RemoveLectureScheduleCommand>
{
    public RemoveLectureScheduleCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.LectureScheduleId).NotNull();

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var lectureSchedulesQueryRepository = context.GetServiceProvider().GetRequiredService<ILectureSchedulesQueryRepository>();

                if (await lectureSchedulesQueryRepository.HasInvalidClassBooksForLectureScheduleAsync(
                        c.SchoolYear!.Value,
                        c.InstId!.Value,
                        c.LectureScheduleId!.Value,
                        ct))
                {
                    context.AddUserFailure(
                        "Лекторския график не може да бъде изтрит, защото има часове, които са от архивиран дневник. " +
                        "Ако желаете да премахнете останалите лекторски часове, моля размаркирайте ги и оставете архивираните за преглед.");
                }

            });
    }
}
