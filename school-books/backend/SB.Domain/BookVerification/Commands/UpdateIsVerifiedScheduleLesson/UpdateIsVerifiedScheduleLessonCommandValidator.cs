namespace SB.Domain;

using System.Linq;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public class UpdateIsVerifiedScheduleLessonCommandValidator : AbstractValidator<UpdateIsVerifiedScheduleLessonCommand>
{
    public UpdateIsVerifiedScheduleLessonCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ScheduleLessons).NotNull();

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var bookVerificationQueryRepository =
                    context.GetServiceProvider().GetRequiredService<IBookVerificationQueryRepository>();

                var existTopics = await bookVerificationQueryRepository.ExistTopicsAsync(
                    c.SchoolYear!.Value,
                    c.ScheduleLessons!.Select(sl => sl.ScheduleLessonId!.Value).ToArray(),
                    ct);

                if (!existTopics)
                {
                    context.AddUnexpectedFailure("Cannot verify lessons that are not taken.");
                }
            });
    }
}
