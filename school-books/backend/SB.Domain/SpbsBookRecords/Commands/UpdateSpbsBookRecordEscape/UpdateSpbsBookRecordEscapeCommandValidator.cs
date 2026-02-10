namespace SB.Domain;

using FluentValidation;

public class UpdateSpbsBookRecordEscapeCommandValidator : AbstractValidator<UpdateSpbsBookRecordEscapeCommand>
{
    public UpdateSpbsBookRecordEscapeCommandValidator(IValidator<CreateSpbsBookRecordEscapeCommand> createValidator)
    {
        this.RuleFor(c => c.OrderNum).NotNull();
        this.RuleFor(c => (CreateSpbsBookRecordEscapeCommand)c).SetValidator(createValidator);
    }
}
