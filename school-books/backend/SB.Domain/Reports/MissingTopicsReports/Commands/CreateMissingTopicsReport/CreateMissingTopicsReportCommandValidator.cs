namespace SB.Domain;

using FluentValidation;

public class CreateMissingTopicsReportCommandValidator : AbstractValidator<CreateMissingTopicsReportCommand>
{
    public CreateMissingTopicsReportCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Period).NotEmpty().IsInEnum();
    }
}
