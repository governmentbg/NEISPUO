namespace SB.Domain;

using FluentValidation;

public class RemoveMissingTopicsReportCommandValidator : AbstractValidator<RemoveMissingTopicsReportCommand>
{
    public RemoveMissingTopicsReportCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.MissingTopicsReportId).NotNull();
    }
}
