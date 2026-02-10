namespace SB.Domain;

using FluentValidation;

public class RemoveAbsencesByStudentsReportCommandValidator : AbstractValidator<RemoveAbsencesByStudentsReportCommand>
{
    public RemoveAbsencesByStudentsReportCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.AbsencesByStudentsReportId).NotNull();
    }
}
