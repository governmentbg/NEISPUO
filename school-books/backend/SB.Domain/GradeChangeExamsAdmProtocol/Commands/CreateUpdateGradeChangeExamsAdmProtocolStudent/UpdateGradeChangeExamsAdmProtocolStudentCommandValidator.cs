namespace SB.Domain;

using FluentValidation;


public class UpdateGradeChangeExamsAdmProtocolStudentCommandValidator : AbstractValidator<UpdateGradeChangeExamsAdmProtocolStudentCommand>
{
    public UpdateGradeChangeExamsAdmProtocolStudentCommandValidator(IValidator<CreateGradeChangeExamsAdmProtocolStudentCommand> createValidator)
    {
        this.RuleFor(s => (CreateGradeChangeExamsAdmProtocolStudentCommand)s).SetValidator(createValidator, "Common");
    }
}
