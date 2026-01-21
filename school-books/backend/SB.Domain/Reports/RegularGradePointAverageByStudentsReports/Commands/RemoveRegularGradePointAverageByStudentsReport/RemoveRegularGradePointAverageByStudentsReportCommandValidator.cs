namespace SB.Domain;

using FluentValidation;

public class RemoveRegularGradePointAverageByStudentsReportCommandValidator : AbstractValidator<RemoveRegularGradePointAverageByStudentsReportCommand>
{
    public RemoveRegularGradePointAverageByStudentsReportCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.RegularGradePointAverageByStudentsReportId).NotNull();
    }
}
