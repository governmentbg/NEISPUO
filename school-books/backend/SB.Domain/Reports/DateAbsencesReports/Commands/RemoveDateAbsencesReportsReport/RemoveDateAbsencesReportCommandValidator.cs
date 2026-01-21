namespace SB.Domain;

using FluentValidation;

public class RemoveDateAbsencesReportCommandValidator : AbstractValidator<RemoveDateAbsencesReportCommand>
{
    public RemoveDateAbsencesReportCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.DateAbsencesReportId).NotNull();
    }
}
