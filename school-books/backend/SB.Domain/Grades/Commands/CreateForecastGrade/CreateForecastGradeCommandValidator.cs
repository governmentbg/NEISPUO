namespace SB.Domain;

using FluentValidation;

public class CreateForecastGradeCommandValidator : AbstractValidator<CreateForecastGradeCommand>
{
    public CreateForecastGradeCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleFor(c => c.PersonId).NotNull();
        this.RuleFor(c => c.CurriculumId).NotNull();
        this.RuleFor(c => c.Category).NotNull();
        this.RuleFor(c => c.Type).NotNull();
        this.RuleFor(c => c.Type)
            .Must(t => t == GradeType.Term || t == GradeType.Final)
            .WithMessage("{PropertyName} must be Term or Final.");
        this.RuleFor(c => c.Term).NotNull();
        this.RuleFor(c => c)
            .Must(c =>
                (c.Category == GradeCategory.Decimal && c.DecimalGrade != null) ||
                (c.Category == GradeCategory.Qualitative && c.QualitativeGrade != null))
            .WithMessage("At least one of DecimalGrade, QualitativeGrade must not be null");
    }
}
