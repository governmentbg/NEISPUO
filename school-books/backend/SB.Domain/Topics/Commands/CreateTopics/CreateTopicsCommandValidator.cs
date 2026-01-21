namespace SB.Domain;

using FluentValidation;

public class CreateTopicsCommandValidator : AbstractValidator<CreateTopicsCommand>
{
    public CreateTopicsCommandValidator(IValidator<CreateTopicsCommandTopic> topicValidator)
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleFor(c => c.Topics).NotEmpty();
        this.RuleForEach(c => c.Topics).SetValidator(topicValidator);
    }
}
