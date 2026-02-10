namespace SB.Domain;

using FluentValidation;

public class RemoveSkillsCheckExamResultProtocolEvaluatorCommandValidator : AbstractValidator<RemoveSkillsCheckExamResultProtocolEvaluatorCommand>
{
    public RemoveSkillsCheckExamResultProtocolEvaluatorCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.SkillsCheckExamResultProtocolId).NotNull();
        this.RuleFor(c => c.SkillsCheckExamResultProtocolEvaluatorId).NotNull();
    }
}
