namespace SB.Domain;

using FluentValidation;

public class CreateAbsenceCommandAbsenceValidator : AbstractValidator<CreateAbsenceCommandAbsence>
{
    public CreateAbsenceCommandAbsenceValidator()
    {
        this.RuleFor(s => s.PersonId).NotNull();
        this.RuleFor(c => c.Date).NotNull();
        this.RuleFor(c => c.ScheduleLessonId).NotNull();

        this.When(c => c.UndoAbsenceId == null, () => {
            this.RuleFor(c => c.Type).NotNull();
        });
    }
}
