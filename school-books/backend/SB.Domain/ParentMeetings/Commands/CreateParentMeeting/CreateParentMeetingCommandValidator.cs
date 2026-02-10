namespace SB.Domain;

using FluentValidation;
using SB.Common;

public class CreateParentMeetingCommandValidator : AbstractValidator<CreateParentMeetingCommand>
{
    public CreateParentMeetingCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Date).NotNull();
        this.RuleFor(c => c.StartTime).NotEmpty().Matches(DateExtensions.HourRegex());
        this.RuleFor(c => c.Location).MaximumLength(100);
        this.RuleFor(c => c.Title).NotEmpty().MaximumLength(100);
        this.RuleFor(c => c.Description).MaximumLength(1000);
    }
}
