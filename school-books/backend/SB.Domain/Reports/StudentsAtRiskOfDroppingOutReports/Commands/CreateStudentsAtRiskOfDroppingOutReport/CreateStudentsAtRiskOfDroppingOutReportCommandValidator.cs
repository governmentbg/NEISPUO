namespace SB.Domain;

using FluentValidation;

public class CreateStudentsAtRiskOfDroppingOutReportCommandValidator : AbstractValidator<CreateStudentsAtRiskOfDroppingOutReportCommand>
{
    public CreateStudentsAtRiskOfDroppingOutReportCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ReportDate).NotEmpty();
    }
}
