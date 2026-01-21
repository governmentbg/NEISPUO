namespace SB.Domain;

using FluentValidation;

public class CreateTopicPlanItemCommandValidator : AbstractValidator<CreateTopicPlanItemCommand>
{
    public CreateTopicPlanItemCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Number).NotNull();
        this.RuleFor(c => c.Title).NotNull();
        this.RuleFor(c => c.Title).MaximumLength(1000);
        this.RuleFor(c => c.Note).MaximumLength(1000);
    }
}
