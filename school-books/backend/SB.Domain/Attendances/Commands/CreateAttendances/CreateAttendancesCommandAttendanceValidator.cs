namespace SB.Domain;

using FluentValidation;

public class CreateAttendancesCommandAttendanceValidator : AbstractValidator<CreateAttendancesCommandAttendance>
{
    public CreateAttendancesCommandAttendanceValidator()
    {
        this.RuleFor(s => s.PersonId).NotNull();
        this.RuleFor(c => c.Type).NotNull();
    }
}
