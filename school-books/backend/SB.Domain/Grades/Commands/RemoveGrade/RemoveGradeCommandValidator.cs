namespace SB.Domain;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public class RemoveGradeCommandValidator : AbstractValidator<RemoveGradeCommand>
{
    public RemoveGradeCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.GradeId).NotNull();

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var gradesQueryRepository = context.GetServiceProvider().GetRequiredService<IGradesQueryRepository>();

                var existsVerifiedScheduleLessonForGrade = await gradesQueryRepository.ExistsVerifiedScheduleLessonForGradeAsync(
                    c.SchoolYear!.Value,
                    c.GradeId!.Value,
                    ct);

                if (existsVerifiedScheduleLessonForGrade)
                {
                    context.AddUserFailure($"Оценкате е въведена за час, който е проверен от директора.");
                }
            });
    }
}
