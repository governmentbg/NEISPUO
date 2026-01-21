namespace SB.Domain;

using FluentValidation;

public class UpdateClassBookStudentGradelessCurriculumsCommandValidator : AbstractValidator<UpdateClassBookStudentGradelessCurriculumsCommand>
{
    public UpdateClassBookStudentGradelessCurriculumsCommandValidator()
    {
        this.RuleFor(s => s.SchoolYear).NotNull();
        this.RuleFor(s => s.InstId).NotNull();
        this.RuleFor(s => s.ClassBookId).NotNull();
        this.RuleFor(s => s.SysUserId).NotNull();

        this.RuleFor(s => s.PersonId).NotNull();
    }
}
