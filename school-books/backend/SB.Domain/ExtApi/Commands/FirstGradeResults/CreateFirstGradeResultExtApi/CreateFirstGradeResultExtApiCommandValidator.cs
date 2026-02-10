namespace SB.Domain;

using FluentValidation;

public class CreateFirstGradeResultExtApiCommandValidator : AbstractValidator<CreateFirstGradeResultExtApiCommand>
{
    public CreateFirstGradeResultExtApiCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleFor(c => c.PersonId).NotNull();
        this.RuleFor(c => c.QualitativeGrade).IsInEnum();
        this.RuleFor(c => c.SpecialGrade).IsInEnum();
        this.RuleFor(c => c)
            .Must(c =>
                (c.QualitativeGrade != null && c.SpecialGrade == null) ||
                (c.SpecialGrade != null && c.QualitativeGrade == null))
            .WithMessage("Exactly one of QualitativeGrade, SpecialGrade must not be null");
    }
}
