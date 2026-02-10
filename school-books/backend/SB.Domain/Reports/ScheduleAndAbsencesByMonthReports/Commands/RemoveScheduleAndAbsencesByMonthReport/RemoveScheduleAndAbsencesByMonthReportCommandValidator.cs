namespace SB.Domain;

using FluentValidation;

public class RemoveScheduleAndAbsencesByMonthReportCommandValidator : AbstractValidator<RemoveScheduleAndAbsencesByMonthReportCommand>
{
    public RemoveScheduleAndAbsencesByMonthReportCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ScheduleAndAbsencesByMonthReportId).NotNull();
    }
}
