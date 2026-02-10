namespace SB.Domain;

using FluentValidation;

public class CreateScheduleAndAbsencesByMonthReportCommandValidator : AbstractValidator<CreateScheduleAndAbsencesByMonthReportCommand>
{
    public CreateScheduleAndAbsencesByMonthReportCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Year).NotNull();
        this.RuleFor(c => c.Month).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
    }
}
