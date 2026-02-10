namespace SB.Domain;

using FluentValidation;

public class RemoveAllClassBookTopicPlanItemsCommandValidator : AbstractValidator<RemoveAllClassBookTopicPlanItemsCommand>
{
    public RemoveAllClassBookTopicPlanItemsCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.CurriculumId).NotNull();
    }
}
