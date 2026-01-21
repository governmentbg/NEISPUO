namespace SB.Domain;

using FluentValidation;

public class CreateSkillsCheckExamResultProtocolCommandValidator : AbstractValidator<CreateSkillsCheckExamResultProtocolCommand>
{
    public CreateSkillsCheckExamResultProtocolCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ProtocolNumber).MaximumLength(100);
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.SubjectId).NotNull();
        this.RuleFor(c => c.StudentsCapacity).NotNull();
    }
}
