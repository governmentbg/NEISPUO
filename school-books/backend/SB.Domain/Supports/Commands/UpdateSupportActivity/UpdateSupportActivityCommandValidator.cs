namespace SB.Domain;

using FluentValidation;

public class UpdateSupportActivityCommandValidator : AbstractValidator<UpdateSupportActivityCommand>
{
    public UpdateSupportActivityCommandValidator(IValidator<CreateSupportActivityCommand> createValidator)
    {
        this.RuleFor(c => c.SupportActivityId).NotNull();
        this.RuleFor(c => (CreateSupportActivityCommand)c).SetValidator(createValidator);
    }
}
