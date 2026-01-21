namespace SB.Domain;
using FluentValidation;

public class UpdateShiftCommandValidator : AbstractValidator<UpdateShiftCommand>
{
    public UpdateShiftCommandValidator(IValidator<CreateShiftCommand> createValidator)
    {
        this.RuleFor(s => (CreateShiftCommand)s).SetValidator(createValidator);
        this.RuleFor(s => s.ShiftId).NotNull();
    }
}
