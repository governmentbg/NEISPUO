namespace SB.Domain;

using FluentValidation;

public class ExcuseAbsencesCommandAbsenceValidator : AbstractValidator<ExcuseAbsencesCommandAbsence>
{
    public ExcuseAbsencesCommandAbsenceValidator()
    {
        this.RuleFor(s => s.AbsenceId).NotNull();
        this.RuleFor(s => s.ScheduleLessonId).NotNull();
    }
}
