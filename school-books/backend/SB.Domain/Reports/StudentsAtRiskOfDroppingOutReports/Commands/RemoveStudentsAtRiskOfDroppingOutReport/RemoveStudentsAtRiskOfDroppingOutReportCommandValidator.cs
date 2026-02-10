namespace SB.Domain;

using FluentValidation;

public class RemoveStudentsAtRiskOfDroppingOutReportCommandValidator : AbstractValidator<RemoveStudentsAtRiskOfDroppingOutReportCommand>
{
    public RemoveStudentsAtRiskOfDroppingOutReportCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.StudentsAtRiskOfDroppingOutReportId).NotNull();
    }
}
