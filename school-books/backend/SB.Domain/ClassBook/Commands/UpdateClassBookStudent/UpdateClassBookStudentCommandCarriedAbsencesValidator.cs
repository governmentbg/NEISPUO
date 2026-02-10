namespace SB.Domain;

using FluentValidation;

// the type is nullable because of this issue:
// https://github.com/FluentValidation/FluentValidation/issues/1648
public class UpdateClassBookStudentCommandCarriedAbsencesValidator : AbstractValidator<UpdateClassBookStudentCommandCarriedAbsences?>
{
    public UpdateClassBookStudentCommandCarriedAbsencesValidator()
    {
        this.RuleFor(s => s!.FirstTermExcusedCount).GreaterThanOrEqualTo(0);
        this.RuleFor(s => s!.FirstTermUnexcusedCount).GreaterThanOrEqualTo(0);
        this.RuleFor(s => s!.FirstTermLateCount).GreaterThanOrEqualTo(0);
        this.RuleFor(s => s!.SecondTermExcusedCount).GreaterThanOrEqualTo(0);
        this.RuleFor(s => s!.SecondTermUnexcusedCount).GreaterThanOrEqualTo(0);
        this.RuleFor(s => s!.SecondTermLateCount).GreaterThanOrEqualTo(0);
    }
}
