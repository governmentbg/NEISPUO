namespace SB.Domain;

using FluentValidation;

public class FinalizeClassBooksCommandValidator : AbstractValidator<FinalizeClassBooksCommand>
{
    public FinalizeClassBooksCommandValidator()
    {
        this.RuleFor(s => s.SchoolYear).NotNull();
        this.RuleFor(s => s.InstId).NotNull();
        this.RuleFor(s => s.SysUserId).NotNull();
        this.RuleFor(s => s.ClassBookIds).NotEmpty();
    }
}
