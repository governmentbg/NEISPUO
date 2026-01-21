namespace SB.Domain;

using FluentValidation;

public class RemoveAllTopicPlanItemsCommandValidator : AbstractValidator<RemoveAllTopicPlanItemsCommand>
{
    public RemoveAllTopicPlanItemsCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.TopicPlanId).NotNull();
    }
}
