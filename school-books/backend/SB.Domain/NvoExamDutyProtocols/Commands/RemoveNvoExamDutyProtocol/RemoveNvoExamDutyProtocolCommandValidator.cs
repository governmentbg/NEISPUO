namespace SB.Domain;

using FluentValidation;

public class RemoveNvoExamDutyProtocolCommandValidator : AbstractValidator<RemoveNvoExamDutyProtocolCommand>
{
    public RemoveNvoExamDutyProtocolCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.NvoExamDutyProtocolId).NotNull();
    }
}
