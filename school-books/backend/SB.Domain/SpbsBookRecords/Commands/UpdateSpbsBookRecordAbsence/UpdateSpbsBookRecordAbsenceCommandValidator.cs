namespace SB.Domain;

using FluentValidation;

public class UpdateSpbsBookRecordAbsenceCommandValidator : AbstractValidator<UpdateSpbsBookRecordAbsenceCommand>
{
    public UpdateSpbsBookRecordAbsenceCommandValidator(IValidator<CreateSpbsBookRecordAbsenceCommand> createValidator)
    {
        this.RuleFor(c => c.OrderNum).NotNull();
        this.RuleFor(c => (CreateSpbsBookRecordAbsenceCommand)c).SetValidator(createValidator);
    }
}
