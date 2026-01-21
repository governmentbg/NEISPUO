namespace SB.Domain;

using FluentValidation;

public class UpdateClassBookStudentCarriedAbsencesCommandValidator : AbstractValidator<UpdateClassBookStudentCarriedAbsencesCommand>
{
    public UpdateClassBookStudentCarriedAbsencesCommandValidator()
    {
        this.RuleFor(s => s.SchoolYear).NotNull();
        this.RuleFor(s => s.InstId).NotNull();
        this.RuleFor(s => s.ClassBookId).NotNull();
        this.RuleFor(s => s.SysUserId).NotNull();

        this.RuleFor(s => s.PersonId).NotNull();
        this.RuleFor(s => s.FirstTermExcusedCount).GreaterThanOrEqualTo(0);
        this.RuleFor(s => s.FirstTermUnexcusedCount).GreaterThanOrEqualTo(0);
        this.RuleFor(s => s.FirstTermLateCount).GreaterThanOrEqualTo(0);
        this.RuleFor(s => s.SecondTermExcusedCount).GreaterThanOrEqualTo(0);
        this.RuleFor(s => s.SecondTermUnexcusedCount).GreaterThanOrEqualTo(0);
        this.RuleFor(s => s.SecondTermLateCount).GreaterThanOrEqualTo(0);
    }
}
