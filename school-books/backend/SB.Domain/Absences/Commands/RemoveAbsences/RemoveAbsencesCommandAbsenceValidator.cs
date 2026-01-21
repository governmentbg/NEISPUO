namespace SB.Domain;

using FluentValidation;

public class RemoveAbsencesCommandAbsenceValidator : AbstractValidator<RemoveAbsencesCommandAbsence>
{
    public RemoveAbsencesCommandAbsenceValidator()
    {
        this.RuleFor(s => s.AbsenceId).NotNull();
        this.RuleFor(s => s.ScheduleLessonId).NotNull();
    }
}
