namespace SB.Domain;

using FluentValidation;

public class CreateConversationCommandValidator : AbstractValidator<CreateConversationCommand>
{
    public CreateConversationCommandValidator(IValidator<CreateConversationCommandParticipant> participantValidator)
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.Title).MaximumLength(2000).NotEmpty();
        this.RuleFor(c => c.Message).MaximumLength(10000).NotEmpty();

        this.RuleFor(c => c.Participants).NotEmpty();
        this.RuleForEach(c => c.Participants).SetValidator(participantValidator);
    }
}
