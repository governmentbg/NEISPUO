namespace SB.Domain;

using FluentValidation;

public class UpdatePublicationCommandValidator : AbstractValidator<UpdatePublicationCommand>
{
    public UpdatePublicationCommandValidator(IValidator<CreatePublicationCommand> createValidator)
    {
        this.RuleFor(c => c.PublicationId).NotNull();
        this.RuleFor(c => (CreatePublicationCommand)c).SetValidator(createValidator);
    }
}
