namespace SB.Domain;

using Data;
using FluentValidation;

public class CreateConversationCommandParticipantValidator : AbstractValidator<CreateConversationCommandParticipant>
{
    public CreateConversationCommandParticipantValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.Title).NotEmpty();
        this.RuleFor(c => c.ParticipantType).NotNull();
        this.RuleFor(c => c.ClassBookId)
            .NotEmpty()
            .When(c =>
                c.ParticipantType is
                    ParticipantType.TeachersForClass or
                    ParticipantType.ParentsForClass);
    }
}
