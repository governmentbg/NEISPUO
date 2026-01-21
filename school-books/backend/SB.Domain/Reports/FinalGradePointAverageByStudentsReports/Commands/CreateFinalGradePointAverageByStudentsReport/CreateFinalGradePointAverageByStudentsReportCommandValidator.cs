namespace SB.Domain;
using FluentValidation;

public class CreateFinalGradePointAverageByStudentsReportCommandValidator : AbstractValidator<CreateFinalGradePointAverageByStudentsReportCommand>
{
    public CreateFinalGradePointAverageByStudentsReportCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Period).NotNull();
    }
}
