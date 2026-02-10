namespace SB.Domain;

using FluentValidation;

public class UpdateClassBookStudentActivitiesCommandValidator : AbstractValidator<UpdateClassBookStudentActivitiesCommand>
{
    public UpdateClassBookStudentActivitiesCommandValidator()
    {
        this.RuleFor(s => s.SchoolYear).NotNull();
        this.RuleFor(s => s.InstId).NotNull();
        this.RuleFor(s => s.ClassBookId).NotNull();
        this.RuleFor(s => s.SysUserId).NotNull();

        this.RuleFor(s => s.PersonId).NotNull();
        this.RuleFor(c => c.Activities).MaximumLength(1000);
    }
}
