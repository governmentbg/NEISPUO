namespace SB.Domain;

using FluentValidation;

public class CreateTopicExtApiCommandValidator : AbstractValidator<CreateTopicExtApiCommand>
{
    public CreateTopicExtApiCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleFor(c => c.Date).NotEmpty();
        this.When(c => c.Title != null, () => {
            this.RuleFor(c => c.Title).NotEmpty().MaximumLength(1000);
            this.RuleFor(c => c.Titles).Null();
        });
        this.When(c => c.Title == null, () => {
            this.RuleFor(c => c.Titles).NotEmpty().ForEach(tt => tt.NotEmpty().MaximumLength(1000));
        });
        this.RuleFor(c => c.ScheduleLessonId).NotEmpty();
    }
}
