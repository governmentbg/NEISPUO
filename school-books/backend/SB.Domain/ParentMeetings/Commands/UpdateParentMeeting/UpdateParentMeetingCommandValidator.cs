namespace SB.Domain;

using FluentValidation;

public class UpdateParentMeetingCommandValidator : AbstractValidator<UpdateParentMeetingCommand>
{
    public UpdateParentMeetingCommandValidator(IValidator<CreateParentMeetingCommand> createValidator)
    {
        this.RuleFor(s => (CreateParentMeetingCommand)s).SetValidator(createValidator);
        this.RuleFor(s => s.ParentMeetingId).NotNull();
    }
}
