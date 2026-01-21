namespace SB.Domain;

using FluentValidation;

public class UpdateSupportCommandExtApiValidator : AbstractValidator<UpdateSupportExtApiCommand>
{
    public UpdateSupportCommandExtApiValidator(IValidator<CreateSupportExtApiCommand> createValidator)
    {
        this.RuleFor(c => c.SupportId).NotNull();
        this.RuleFor(c => (CreateSupportExtApiCommand)c).SetValidator(createValidator);
    }
}
