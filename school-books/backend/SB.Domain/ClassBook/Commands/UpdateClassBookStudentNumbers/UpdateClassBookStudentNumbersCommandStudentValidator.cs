namespace SB.Domain;

using FluentValidation;

public class UpdateClassBookStudentNumbersCommandStudentValidator : AbstractValidator<UpdateClassBookStudentNumbersCommandStudent>
{
    public UpdateClassBookStudentNumbersCommandStudentValidator()
    {
        this.RuleFor(c => c.PersonId).NotNull();
    }
}
