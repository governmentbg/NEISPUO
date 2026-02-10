namespace SB.Domain;

using FluentValidation;

public class RemoveAbsencesByClassesReportCommandValidator : AbstractValidator<RemoveAbsencesByClassesReportCommand>
{
    public RemoveAbsencesByClassesReportCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.AbsencesByClassesReportId).NotNull();
    }
}
