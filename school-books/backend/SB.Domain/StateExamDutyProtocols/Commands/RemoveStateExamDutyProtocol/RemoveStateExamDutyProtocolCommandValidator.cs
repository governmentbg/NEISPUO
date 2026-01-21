namespace SB.Domain;

using FluentValidation;

public class RemoveStateExamDutyProtocolCommandValidator : AbstractValidator<RemoveStateExamDutyProtocolCommand>
{
    public RemoveStateExamDutyProtocolCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.StateExamDutyProtocolId).NotNull();
    }
}
