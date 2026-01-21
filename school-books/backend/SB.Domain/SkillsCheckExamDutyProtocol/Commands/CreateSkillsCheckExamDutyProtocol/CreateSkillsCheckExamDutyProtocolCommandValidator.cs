namespace SB.Domain;

using FluentValidation;

public class CreateSkillsCheckExamDutyProtocolCommandValidator : AbstractValidator<CreateSkillsCheckExamDutyProtocolCommand>
{
    public CreateSkillsCheckExamDutyProtocolCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ProtocolNumber).MaximumLength(100);
        this.RuleFor(c => c.SubjectId).NotNull();
        this.RuleFor(c => c.SubjectTypeId).NotNull();
        this.RuleFor(c => c.Date).NotEmpty();
        this.RuleFor(c => c.StudentsCapacity).NotNull();
        this.RuleFor(c => c.SupervisorPersonIds).NotEmpty();
    }
}
