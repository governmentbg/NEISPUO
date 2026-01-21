namespace SB.Domain;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

internal class RemoveTopicDplrCommandValidator : AbstractValidator<RemoveTopicDplrCommand>
{
    public RemoveTopicDplrCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(s => s.TopicDplrId).NotNull();

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var topicsQueryRepository = context.GetServiceProvider().GetRequiredService<ITopicsDplrQueryRepository>();

                var existsVerifiedScheduleLessonForAbsences = await topicsQueryRepository.ExistsVerifiedTopicDplrAsync(
                    c.SchoolYear!.Value,
                    c.TopicDplrId!.Value,
                    ct);

                if (existsVerifiedScheduleLessonForAbsences)
                {
                    context.AddUserFailure($"Не може да се изтрие тема въведена за час, който е проверен от директора.");
                }
            });
    }
}
