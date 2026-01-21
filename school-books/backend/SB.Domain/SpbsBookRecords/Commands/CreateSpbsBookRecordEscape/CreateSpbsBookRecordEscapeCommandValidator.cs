namespace SB.Domain;

using FluentValidation;
using SB.Common;

public class CreateSpbsBookRecordEscapeCommandValidator : AbstractValidator<CreateSpbsBookRecordEscapeCommand>
{
    public CreateSpbsBookRecordEscapeCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.SpbsBookRecordId).NotNull();

        this.RuleFor(c => c.EscapeDate).NotEmpty();
        this.RuleFor(c => c.EscapeTime).NotEmpty().Matches(DateExtensions.HourRegex());
        this.RuleFor(c => c.PoliceNotificationDate).NotEmpty();
        this.RuleFor(c => c.PoliceNotificationTime).NotEmpty().Matches(DateExtensions.HourRegex());
        this.RuleFor(c => c.PoliceLetterNumber).NotEmpty().MaximumLength(100);
        this.RuleFor(c => c.PoliceLetterDate).NotEmpty();
    }
}
