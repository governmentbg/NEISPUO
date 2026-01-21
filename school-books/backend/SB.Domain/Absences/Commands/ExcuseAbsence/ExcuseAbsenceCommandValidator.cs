namespace SB.Domain;

using FluentValidation;

public class ExcuseAbsenceCommandValidator : AbstractValidator<ExcuseAbsenceCommand>
{
    public ExcuseAbsenceCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.AbsenceId).NotNull();
        this.RuleFor(c => c.ExcusedReasonComment).MaximumLength(1000);
    }
}
