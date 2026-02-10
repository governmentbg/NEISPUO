namespace SB.Domain;

using FluentValidation;

public class UpdateFirstGradeResultCommandStudentValidator : AbstractValidator<UpdateFirstGradeResultCommandStudent>
{
    public UpdateFirstGradeResultCommandStudentValidator()
    {
        this.RuleFor(c => c.PersonId).NotNull();
    }
}
