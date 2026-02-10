namespace SB.Domain;

using FluentValidation;
using SB.Common;

public class CreateShiftCommandHourValidator : AbstractValidator<CreateShiftCommandHour>
{
    public CreateShiftCommandHourValidator()
    {
        this.RuleFor(c => c.HourNumber).NotNull().GreaterThanOrEqualTo(0);
        this.RuleFor(c => c.StartTime).NotEmpty().Matches(DateExtensions.HourRegex());
        this.RuleFor(c => c.EndTime).NotEmpty().Matches(DateExtensions.HourRegex());
    }
}
