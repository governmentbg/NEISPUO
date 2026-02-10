namespace SB.Domain;

using FluentValidation;

public class ExcuseAbsencesCommandValidator : AbstractValidator<ExcuseAbsencesCommand>
{
    public ExcuseAbsencesCommandValidator(IValidator<ExcuseAbsencesCommandAbsence> absenceValidator)
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleFor(c => c.Absences).NotEmpty();
        this.RuleForEach(c => c.Absences).SetValidator(absenceValidator);

        this.RuleFor(c => c.ExcusedReasonComment).MaximumLength(1000);
    }
}
