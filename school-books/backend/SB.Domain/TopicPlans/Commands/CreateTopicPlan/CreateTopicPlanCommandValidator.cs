namespace SB.Domain;

using FluentValidation;

public class CreateTopicPlanCommandValidator : AbstractValidator<CreateTopicPlanCommand>
{
    public CreateTopicPlanCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Name).NotNull();
        this.RuleFor(c => c.Name).MaximumLength(100);
    }
}
