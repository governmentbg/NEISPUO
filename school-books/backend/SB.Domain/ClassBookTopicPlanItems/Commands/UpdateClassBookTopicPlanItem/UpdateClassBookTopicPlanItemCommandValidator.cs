namespace SB.Domain;

using FluentValidation;

public class UpdateClassBookTopicPlanItemCommandValidator : AbstractValidator<UpdateClassBookTopicPlanItemCommand>
{
    public UpdateClassBookTopicPlanItemCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ClassBookTopicPlanItemId).NotNull();
        this.RuleFor(c => c.CurriculumId).NotNull();
        this.RuleFor(c => c.Number).NotNull();
        this.RuleFor(c => c.Title).NotNull().MaximumLength(1000);
        this.RuleFor(c => c.Taken).NotNull();
        this.RuleFor(c => c.Note).MaximumLength(1000);
    }
}
