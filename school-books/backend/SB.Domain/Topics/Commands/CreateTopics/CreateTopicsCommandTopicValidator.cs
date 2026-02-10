namespace SB.Domain;

using FluentValidation;

public class CreateTopicsCommandTopicValidator : AbstractValidator<CreateTopicsCommandTopic>
{
    public CreateTopicsCommandTopicValidator()
    {
        this.When(c => c.Title != null, () =>
        {
            this.RuleFor(s => s.Title).NotEmpty().MaximumLength(1000);
            this.RuleFor(c => c.ClassBookTopicPlanItemIds).Null();
        });
        this.When(c => c.ClassBookTopicPlanItemIds != null, () =>
        {
            this.RuleFor(s => s.Title).Null();
            this.RuleFor(c => c.ClassBookTopicPlanItemIds).NotEmpty();
        });
        this.RuleFor(c => c.Date).NotNull();
        this.RuleFor(c => c.ScheduleLessonId).NotNull();
    }
}
