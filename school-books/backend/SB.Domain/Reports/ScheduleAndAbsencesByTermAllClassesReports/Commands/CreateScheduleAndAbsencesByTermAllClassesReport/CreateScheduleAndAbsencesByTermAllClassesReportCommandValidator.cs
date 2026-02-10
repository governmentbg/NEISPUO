namespace SB.Domain;

using FluentValidation;

public class CreateScheduleAndAbsencesByTermAllClassesReportCommandValidator : AbstractValidator<CreateScheduleAndAbsencesByTermAllClassesReportCommand>
{
    public CreateScheduleAndAbsencesByTermAllClassesReportCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Term).NotEmpty().IsInEnum();
    }
}
