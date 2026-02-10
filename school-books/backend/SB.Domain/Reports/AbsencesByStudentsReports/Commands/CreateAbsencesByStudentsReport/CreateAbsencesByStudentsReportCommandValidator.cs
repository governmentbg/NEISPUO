namespace SB.Domain;

using FluentValidation;

public class CreateAbsencesByStudentsReportCommandValidator : AbstractValidator<CreateAbsencesByStudentsReportCommand>
{
    public CreateAbsencesByStudentsReportCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Period).NotNull();
    }
}
