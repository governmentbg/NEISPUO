namespace SB.Domain;

using FluentValidation;

public class CreateSkillsCheckExamResultProtocolEvaluatorCommandValidator : AbstractValidator<CreateSkillsCheckExamResultProtocolEvaluatorCommand>
{
    public CreateSkillsCheckExamResultProtocolEvaluatorCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.SkillsCheckExamResultProtocolId).NotNull();
        this.RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
    }
}
