namespace SB.Domain;

using FluentValidation;

public class RemoveGraduationThesisDefenseProtocolCommandValidator : AbstractValidator<RemoveGraduationThesisDefenseProtocolCommand>
{
    public RemoveGraduationThesisDefenseProtocolCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.GraduationThesisDefenseProtocolId).NotNull();
    }
}
