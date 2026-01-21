namespace SB.Domain;

using FluentValidation;

public class UpdateTakenClassBookTopicPlanItemCommandValidator : AbstractValidator<UpdateTakenClassBookTopicPlanItemCommand>
{
    public UpdateTakenClassBookTopicPlanItemCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ClassBookTopicPlanItemId).NotNull();
        this.RuleFor(c => c.CurriculumId).NotNull();
        this.RuleFor(c => c.Taken).NotNull();
    }
}
