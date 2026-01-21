namespace SB.Domain;

using System.Linq;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public class RemoveTopicsCommandValidator : AbstractValidator<RemoveTopicsCommand>
{
    public RemoveTopicsCommandValidator(IValidator<RemoveTopicsCommandTopic> topicValidator)
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleFor(c => c.Topics).NotEmpty();
        this.RuleForEach(c => c.Topics).SetValidator(topicValidator);

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var topicsQueryRepository = context.GetServiceProvider().GetRequiredService<ITopicsQueryRepository>();

                var existsVerifiedScheduleLessonForAbsences = await topicsQueryRepository.ExistsVerifiedScheduleLessonForTopicsAsync(
                    c.SchoolYear!.Value,
                    c.Topics!.Select(a => a.TopicId!.Value).ToArray(),
                    ct);

                if (existsVerifiedScheduleLessonForAbsences)
                {
                    context.AddUserFailure($"Не може да се изтрие тема въведена за час, който е проверен от директора.");
                }
            });
    }
}
