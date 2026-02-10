namespace SB.Domain;

using FluentValidation;

public class CreateShiftCommandValidator : AbstractValidator<CreateShiftCommand>
{
    public CreateShiftCommandValidator(IValidator<CreateShiftCommandDay> datValidator)
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
        this.RuleFor(c => c.IsMultiday).NotNull();
        this.RuleFor(c => c.Days).NotEmpty();
        this.RuleForEach(c => c.Days).SetValidator(datValidator);
    }
}
