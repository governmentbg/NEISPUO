namespace SB.Domain;

using FluentValidation;

public class UpdateGradeChangeExamsAdmProtocolCommandValidator : AbstractValidator<UpdateGradeChangeExamsAdmProtocolCommand>
{
    public UpdateGradeChangeExamsAdmProtocolCommandValidator(IValidator<CreateGradeChangeExamsAdmProtocolCommand> createValidator)
    {
        this.RuleFor(s => (CreateGradeChangeExamsAdmProtocolCommand)s).SetValidator(createValidator);
        this.RuleFor(s => s.GradeChangeExamsAdmProtocolId).NotNull();
    }
}
