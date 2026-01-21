namespace SB.Domain;

using FluentValidation;

public class CreateShiftCommandDayValidator : AbstractValidator<CreateShiftCommandDay>
{
    public CreateShiftCommandDayValidator(IValidator<CreateShiftCommandHour> hourValidator)
    {
        this.RuleFor(c => c.Day).NotNull().GreaterThanOrEqualTo(1).LessThanOrEqualTo(7);
        this.RuleFor(c => c.Hours).NotNull();
        this.RuleForEach(c => c.Hours).SetValidator(hourValidator);
    }
}
