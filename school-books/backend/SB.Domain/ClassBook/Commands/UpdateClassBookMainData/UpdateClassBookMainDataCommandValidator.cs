namespace SB.Domain;

using FluentValidation;

public class UpdateClassBookMainDataCommandValidator : AbstractValidator<UpdateClassBookMainDataCommand>
{
    public UpdateClassBookMainDataCommandValidator()
    {
        this.RuleFor(s => s.SchoolYear).NotNull();
        this.RuleFor(s => s.InstId).NotNull();
        this.RuleFor(s => s.ClassBookId).NotNull();
        this.RuleFor(s => s.SysUserId).NotNull();
        this.RuleFor(c => c.BookName).MaximumLength(300);
    }
}
