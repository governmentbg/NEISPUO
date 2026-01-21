namespace SB.Domain;

using FluentValidation;

public class UpdateScheduleCommandValidator : AbstractValidator<UpdateScheduleCommand>
{
    public UpdateScheduleCommandValidator(IValidator<CreateScheduleCommand> createValidator)
    {
        this.RuleFor(s => (CreateScheduleCommand)s).SetValidator(createValidator);
        this.RuleFor(s => s.ScheduleId).NotNull();
    }
}
