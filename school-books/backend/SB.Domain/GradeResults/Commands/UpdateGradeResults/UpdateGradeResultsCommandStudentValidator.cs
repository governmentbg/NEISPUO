namespace SB.Domain;

using FluentValidation;

public class UpdateGradeResultsCommandStudentValidator : AbstractValidator<UpdateGradeResultsCommandStudent>
{
    public UpdateGradeResultsCommandStudentValidator()
    {
        this.RuleFor(s => s.PersonId).NotNull();
    }
}
