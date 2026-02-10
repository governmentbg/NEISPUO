namespace SB.Domain;

using FluentValidation;

public class RemoveGradelessStudentsReportCommandValidator : AbstractValidator<RemoveGradelessStudentsReportCommand>
{
    public RemoveGradelessStudentsReportCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.GradelessStudentsReportId).NotNull();
    }
}
