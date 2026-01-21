namespace SB.Domain;

using FluentValidation;

public class UpdateTopicPlanCommandValidator : AbstractValidator<UpdateTopicPlanCommand>
{
    public UpdateTopicPlanCommandValidator()
    {
        this.RuleFor(c => c.TopicPlanId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Name).NotNull();
        this.RuleFor(c => c.Name).MaximumLength(100);
    }
}
