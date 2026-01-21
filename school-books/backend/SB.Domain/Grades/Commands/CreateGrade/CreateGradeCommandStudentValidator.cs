namespace SB.Domain;

using FluentValidation;

public class CreateGradeCommandStudentValidator : AbstractValidator<CreateGradeCommandStudent>
{
    public CreateGradeCommandStudentValidator()
    {
        this.RuleFor(s => s.PersonId).NotNull();
        this.RuleFor(s => s.DecimalGrade)
            .Must(g => g == null || g == 2 || (g >= 3 && g <= 6))
            .WithMessage("{PropertyName} must be a valid decimal grade.");
        this.RuleFor(s => s)
            .Must(s =>
                s.DecimalGrade != null ||
                s.QualitativeGrade != null ||
                s.SpecialGrade != null)
            .WithMessage("At least one of DecimalGrade, QualitativeGrade, SpecialGrade must not be null.");
        this.RuleFor(c => c.Comment).MaximumLength(1000);
    }
}
