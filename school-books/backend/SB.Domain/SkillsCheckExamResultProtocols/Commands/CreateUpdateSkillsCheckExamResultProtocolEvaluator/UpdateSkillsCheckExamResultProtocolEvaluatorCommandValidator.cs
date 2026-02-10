namespace SB.Domain;

using FluentValidation;


public class UpdateSkillsCheckExamResultProtocolEvaluatorCommandValidator : AbstractValidator<UpdateSkillsCheckExamResultProtocolEvaluatorCommand>
{
    public UpdateSkillsCheckExamResultProtocolEvaluatorCommandValidator(IValidator<CreateSkillsCheckExamResultProtocolEvaluatorCommand> createValidator)
    {
        this.RuleFor(s => (CreateSkillsCheckExamResultProtocolEvaluatorCommand)s).SetValidator(createValidator);
    }
}
