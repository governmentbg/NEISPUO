namespace SB.Domain;

using FluentValidation;

public class RemoveSupportCommandValidator : AbstractValidator<RemoveSupportCommand>
{
    public RemoveSupportCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.SupportId).NotNull();
    }
}
