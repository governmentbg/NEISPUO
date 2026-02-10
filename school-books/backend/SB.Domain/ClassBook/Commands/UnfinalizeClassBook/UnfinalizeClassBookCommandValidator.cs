namespace SB.Domain;

using FluentValidation;

public class UnfinalizeClassBookCommandValidator : AbstractValidator<UnfinalizeClassBookCommand>
{
    public UnfinalizeClassBookCommandValidator()
    {
        this.RuleFor(s => s.SchoolYear).NotNull();
        this.RuleFor(s => s.InstId).NotNull();
        this.RuleFor(s => s.ClassBookId).NotNull();
        this.RuleFor(s => s.SysUserId).NotNull();
    }
}
