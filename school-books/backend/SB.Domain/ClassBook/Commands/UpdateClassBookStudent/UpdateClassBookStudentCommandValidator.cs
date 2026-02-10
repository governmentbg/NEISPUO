namespace SB.Domain;

using FluentValidation;

public class UpdateClassBookStudentCommandValidator : AbstractValidator<UpdateClassBookStudentCommand>
{
    public UpdateClassBookStudentCommandValidator(
        IValidator<UpdateClassBookStudentCommandGradeless> gradelessCurriculumValidator,
        IValidator<UpdateClassBookStudentCommandCarriedAbsences?> carriedAbsenceValidator)
    {
        this.RuleFor(s => s.SchoolYear).NotNull();
        this.RuleFor(s => s.InstId).NotNull();
        this.RuleFor(s => s.ClassBookId).NotNull();
        this.RuleFor(s => s.SysUserId).NotNull();

        this.RuleFor(s => s.PersonId).NotNull();
        this.RuleFor(c => c.SpecialNeedCurriculumIds).NotNull();
        this.RuleFor(c => c.GradelessCurriculums).NotNull();
        this.RuleForEach(c => c.GradelessCurriculums).SetValidator(gradelessCurriculumValidator);
        this.RuleFor(c => c.Activities).MaximumLength(1000);
        this.RuleFor(c => c.CarriedAbsences).SetValidator(carriedAbsenceValidator);
    }
}
