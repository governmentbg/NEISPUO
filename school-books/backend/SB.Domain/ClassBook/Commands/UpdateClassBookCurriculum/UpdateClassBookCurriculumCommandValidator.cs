namespace SB.Domain;

using FluentValidation;

public class UpdateClassBookCurriculumCommandValidator : AbstractValidator<UpdateClassBookCurriculumCommand>
{
    public UpdateClassBookCurriculumCommandValidator()
    {
        this.RuleFor(s => s.SchoolYear).NotNull();
        this.RuleFor(s => s.InstId).NotNull();
        this.RuleFor(s => s.ClassBookId).NotNull();
        this.RuleFor(s => s.SysUserId).NotNull();
        this.RuleFor(s => s.CurriculumId).NotNull();
        this.RuleFor(s => s.WithoutGrade).NotNull();
    }
}
