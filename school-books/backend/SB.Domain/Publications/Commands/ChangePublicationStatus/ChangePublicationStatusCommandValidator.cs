namespace SB.Domain;

using FluentValidation;

public class ChangePublicationStatusCommandValidator : AbstractValidator<ChangePublicationStatusCommand>
{
    public ChangePublicationStatusCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.PublicationId).NotNull();
        this.RuleFor(c => c.Status).NotEmpty().IsInEnum();
    }
}
