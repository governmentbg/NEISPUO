namespace SB.Domain;

using FluentValidation;

public class CreateDateAbsencesReportCommandValidator : AbstractValidator<CreateDateAbsencesReportCommand>
{
    public CreateDateAbsencesReportCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ReportDate).NotNull();
        this.RuleFor(c => c.IsUnited).NotNull();
    }
}
