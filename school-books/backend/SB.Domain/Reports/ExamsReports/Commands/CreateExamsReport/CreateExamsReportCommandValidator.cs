namespace SB.Domain;

using FluentValidation;

public class CreateExamsReportCommandValidator : AbstractValidator<CreateExamsReportCommand>
{
    public CreateExamsReportCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
    }
}
