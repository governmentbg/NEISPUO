namespace SB.Domain;

using FluentValidation;

public class CreateScheduleAndAbsencesByTermReportCommandValidator : AbstractValidator<CreateScheduleAndAbsencesByTermReportCommand>
{
    public CreateScheduleAndAbsencesByTermReportCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Term).NotEmpty().IsInEnum();
        this.RuleFor(c => c.ClassBookId).NotNull();
    }
}
