namespace SB.Domain;

using FluentValidation;

public class AddConversationMessageCommandValidator : AbstractValidator<AddConversationMessageCommand>
{
    public AddConversationMessageCommandValidator()
    {
        this.RuleFor(p => p.SchoolYear).NotNull();
        this.RuleFor(p => p.ConversationId).NotNull();
        this.RuleFor(p => p.Message).MaximumLength(10000).NotEmpty();
    }
}
