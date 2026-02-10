namespace SB.Domain;

using FluentValidation;

public class RemoveFinalGradePointAverageByStudentsReportCommandValidator : AbstractValidator<RemoveFinalGradePointAverageByStudentsReportCommand>
{
    public RemoveFinalGradePointAverageByStudentsReportCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.FinalGradePointAverageByStudentsReportId).NotNull();
    }
}
