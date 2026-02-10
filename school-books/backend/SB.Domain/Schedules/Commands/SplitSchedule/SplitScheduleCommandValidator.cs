namespace SB.Domain;

using FluentValidation;

public class SplitScheduleCommandValidator : AbstractValidator<SplitScheduleCommand>
{
    public SplitScheduleCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleFor(s => s.ScheduleId).NotNull();
        this.RuleFor(c => c.Weeks).NotEmpty();
    }
}
