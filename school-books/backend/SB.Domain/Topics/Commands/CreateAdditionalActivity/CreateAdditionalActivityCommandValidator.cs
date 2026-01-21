namespace SB.Domain;

using FluentValidation;

public class CreateAdditionalActivityCommandValidator : AbstractValidator<CreateAdditionalActivityCommand>
{
    public CreateAdditionalActivityCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Year).NotNull();
        this.RuleFor(c => c.WeekNumber).NotNull();
        this.RuleFor(c => c.Activity).NotEmpty().MaximumLength(10000);
    }
}
