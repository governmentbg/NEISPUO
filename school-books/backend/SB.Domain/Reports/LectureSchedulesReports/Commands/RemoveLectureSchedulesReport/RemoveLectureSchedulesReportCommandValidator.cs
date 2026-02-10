namespace SB.Domain;

using FluentValidation;

public class RemoveLectureSchedulesReportCommandValidator : AbstractValidator<RemoveLectureSchedulesReportCommand>
{
    public RemoveLectureSchedulesReportCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.LectureSchedulesReportId).NotNull();
    }
}
