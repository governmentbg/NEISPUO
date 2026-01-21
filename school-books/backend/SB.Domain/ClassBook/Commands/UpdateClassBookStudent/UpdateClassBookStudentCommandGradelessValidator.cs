namespace SB.Domain;

using FluentValidation;

public class UpdateClassBookStudentCommandGradelessValidator : AbstractValidator<UpdateClassBookStudentCommandGradeless>
{
    public UpdateClassBookStudentCommandGradelessValidator()
    {
        this.RuleFor(s => s.CurriculumId).NotNull();
        this.RuleFor(s => s.WithoutFirstTermGrade).NotNull();
        this.RuleFor(s => s.WithoutSecondTermGrade).NotNull();
        this.RuleFor(s => s.WithoutFinalGrade).NotNull();
    }
}
