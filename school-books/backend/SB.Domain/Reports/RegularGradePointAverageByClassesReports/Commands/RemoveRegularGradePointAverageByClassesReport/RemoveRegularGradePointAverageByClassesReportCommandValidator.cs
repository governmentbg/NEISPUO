namespace SB.Domain;

using FluentValidation;

public class RemoveRegularGradePointAverageByClassesReportCommandValidator : AbstractValidator<RemoveRegularGradePointAverageByClassesReportCommand>
{
    public RemoveRegularGradePointAverageByClassesReportCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.RegularGradePointAverageByClassesReportId).NotNull();
    }
}
