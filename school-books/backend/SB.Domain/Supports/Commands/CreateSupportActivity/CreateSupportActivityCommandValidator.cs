namespace SB.Domain;

using FluentValidation;

public class CreateSupportActivityCommandValidator : AbstractValidator<CreateSupportActivityCommand>
{
    public CreateSupportActivityCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.SupportActivityTypeId).NotEmpty();
        this.RuleFor(c => c.Target).MaximumLength(10000);
        this.RuleFor(c => c.Result).MaximumLength(10000);
    }
}
