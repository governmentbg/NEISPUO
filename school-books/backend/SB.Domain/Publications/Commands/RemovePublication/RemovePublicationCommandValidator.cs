namespace SB.Domain;

using FluentValidation;

public class RemovePublicationCommandValidator : AbstractValidator<RemovePublicationCommand>
{
    public RemovePublicationCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.PublicationId).NotNull();
    }
}
