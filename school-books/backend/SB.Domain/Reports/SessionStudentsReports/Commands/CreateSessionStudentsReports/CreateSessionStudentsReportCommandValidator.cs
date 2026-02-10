namespace SB.Domain;

using FluentValidation;

public class CreateSessionStudentsReportCommandValidator : AbstractValidator<CreateSessionStudentsReportCommand>
{
    public CreateSessionStudentsReportCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
    }
}
