namespace SB.Domain;

using FluentValidation;

public class ExcuseAttendanceCommandValidator : AbstractValidator<ExcuseAttendanceCommand>
{
    public ExcuseAttendanceCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.AttendanceId).NotNull();
        this.RuleFor(c => c.ExcusedReasonComment).MaximumLength(1000);
    }
}
