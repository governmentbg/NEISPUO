namespace SB.Domain;

using FluentValidation;

public class UpdateSupportCommandValidator : AbstractValidator<UpdateSupportCommand>
{
    public UpdateSupportCommandValidator(IValidator<CreateSupportCommand> createValidator)
    {
        this.RuleFor(c => c.SupportId).NotNull();
        this.RuleFor(c => (CreateSupportCommand)c).SetValidator(createValidator);
    }
}
