namespace SB.Domain;

using FluentValidation;

public class RemoveSkillsCheckExamResultProtocolCommandValidator : AbstractValidator<RemoveSkillsCheckExamResultProtocolCommand>
{
    public RemoveSkillsCheckExamResultProtocolCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.SkillsCheckExamResultProtocolId).NotNull();
    }
}
