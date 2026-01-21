namespace SB.Domain;

using FluentValidation;

public class CreateGradelessStudentsReportCommandValidator : AbstractValidator<CreateGradelessStudentsReportCommand>
{
    public CreateGradelessStudentsReportCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.OnlyFinalGrades).NotNull();
        this.RuleFor(c => c.Period).NotNull();
    }
}
