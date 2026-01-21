namespace SB.Domain;

using FluentValidation;

public class CreateTeacherAbsenceCommandHourValidator : AbstractValidator<CreateTeacherAbsenceCommandHour>
{
    public CreateTeacherAbsenceCommandHourValidator()
    {
        this.RuleFor(s => s.ScheduleLessonId).NotNull();
    }
}
