namespace SB.Domain;

using FluentValidation;

public class SignClassBookPrintCommandValidator : AbstractValidator<SignClassBookPrintCommand>
{
    public SignClassBookPrintCommandValidator()
    {
        this.RuleFor(s => s.SchoolYear).NotNull();
        this.RuleFor(s => s.InstId).NotNull();
        this.RuleFor(s => s.SysUserId).NotNull();
        this.RuleFor(s => s.ClassBookId).NotNull();
        this.RuleFor(s => s.ClassBookPrintId).NotNull();
        this.RuleFor(s => s.SignedClassBookPrintFileBase64).NotEmpty();
    }
}
