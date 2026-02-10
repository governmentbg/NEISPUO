namespace SB.Domain;

using FluentValidation;

public class RemoveExamsReportCommandValidator : AbstractValidator<RemoveExamsReportCommand>
{
    public RemoveExamsReportCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ExamsReportId).NotNull();
    }
}
