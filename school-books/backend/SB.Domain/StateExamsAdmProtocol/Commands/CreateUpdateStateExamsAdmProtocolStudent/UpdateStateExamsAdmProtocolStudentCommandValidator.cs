namespace SB.Domain;

using FluentValidation;


public class UpdateStateExamsAdmProtocolStudentCommandValidator : AbstractValidator<UpdateStateExamsAdmProtocolStudentCommand>
{
    public UpdateStateExamsAdmProtocolStudentCommandValidator(IValidator<CreateStateExamsAdmProtocolStudentCommand> createValidator)
    {
        this.RuleFor(s => (CreateStateExamsAdmProtocolStudentCommand)s).SetValidator(createValidator, "Common");
    }
}
