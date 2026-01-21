namespace SB.Domain;
using FluentValidation;

public class CreateFinalGradePointAverageByClassesReportCommandValidator : AbstractValidator<CreateFinalGradePointAverageByClassesReportCommand>
{
    public CreateFinalGradePointAverageByClassesReportCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Period).NotNull();
    }
}
