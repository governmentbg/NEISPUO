namespace SB.Domain;

using FluentValidation;

public class UpdateStateExamsAdmProtocolCommandValidator : AbstractValidator<UpdateStateExamsAdmProtocolCommand>
{
    public UpdateStateExamsAdmProtocolCommandValidator(IValidator<CreateStateExamsAdmProtocolCommand> createValidator)
    {
        this.RuleFor(s => (CreateStateExamsAdmProtocolCommand)s).SetValidator(createValidator);
        this.RuleFor(s => s.StateExamsAdmProtocolId).NotNull();
    }
}
