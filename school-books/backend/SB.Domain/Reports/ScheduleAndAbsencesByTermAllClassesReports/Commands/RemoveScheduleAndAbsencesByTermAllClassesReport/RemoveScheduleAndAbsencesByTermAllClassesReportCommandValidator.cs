namespace SB.Domain;

using FluentValidation;

public class RemoveScheduleAndAbsencesByTermAllClassesReportCommandValidator : AbstractValidator<RemoveScheduleAndAbsencesByTermAllClassesReportCommand>
{
    public RemoveScheduleAndAbsencesByTermAllClassesReportCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ScheduleAndAbsencesByTermAllClassesReportId).NotNull();
    }
}
