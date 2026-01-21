namespace SB.Domain;

using FluentValidation;

public class UpdateFirstGradeResultExtApiCommandValidator : AbstractValidator<UpdateFirstGradeResultExtApiCommand>
{
    public UpdateFirstGradeResultExtApiCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.FirstGradeResultId).NotNull();

        this.RuleFor(c => c.QualitativeGrade).IsInEnum();
        this.RuleFor(c => c.SpecialGrade).IsInEnum();
    }
}
