namespace SB.Domain;

using FluentValidation;

public class RemoveTopicPlanItemCommandValidator : AbstractValidator<RemoveTopicPlanItemCommand>
{
    public RemoveTopicPlanItemCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.TopicPlanItemId).NotNull();
    }
}
