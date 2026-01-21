namespace SB.Domain;

using FluentValidation;

public class RemoveScheduleAndAbsencesByTermReportCommandValidator : AbstractValidator<RemoveScheduleAndAbsencesByTermReportCommand>
{
    public RemoveScheduleAndAbsencesByTermReportCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ScheduleAndAbsencesByTermReportId).NotNull();
    }
}
