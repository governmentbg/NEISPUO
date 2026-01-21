namespace SB.Domain;

using FluentValidation;

public class UpdateAdditionalActivityCommandValidator : AbstractValidator<UpdateAdditionalActivityCommand>
{
    public UpdateAdditionalActivityCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Activity).NotEmpty().MaximumLength(10000);
        this.RuleFor(c => c.AdditionalActivityId).NotNull();
    }
}
