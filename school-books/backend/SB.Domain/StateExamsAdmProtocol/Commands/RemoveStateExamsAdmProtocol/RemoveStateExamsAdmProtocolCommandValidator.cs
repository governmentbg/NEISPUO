namespace SB.Domain;

using FluentValidation;

public class RemoveStateExamsAdmProtocolCommandValidator: AbstractValidator<RemoveStateExamsAdmProtocolCommand>
{
    public RemoveStateExamsAdmProtocolCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.StateExamsAdmProtocolId).NotNull();
    }
}
