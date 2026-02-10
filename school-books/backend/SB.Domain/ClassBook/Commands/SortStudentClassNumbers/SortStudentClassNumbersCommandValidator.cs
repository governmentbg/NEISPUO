namespace SB.Domain;

using FluentValidation;

public class SortStudentClassNumbersCommandValidator : AbstractValidator<SortStudentClassNumbersCommand>
{
    public SortStudentClassNumbersCommandValidator()
    {
        this.RuleFor(s => s.SchoolYear).NotNull();
        this.RuleFor(s => s.InstId).NotNull();
        this.RuleFor(s => s.ClassBookId).NotNull();
        this.RuleFor(s => s.SysUserId).NotNull();
    }
}
