namespace SB.Domain;

using FluentValidation;

public class UpdateFirstGradeResultCommandValidator : AbstractValidator<UpdateFirstGradeResultCommand>
{
    public UpdateFirstGradeResultCommandValidator(IValidator<UpdateFirstGradeResultCommandStudent> studentValidator)
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Students).NotNull();
        this.RuleForEach(c => c.Students).SetValidator(studentValidator);
    }
}
