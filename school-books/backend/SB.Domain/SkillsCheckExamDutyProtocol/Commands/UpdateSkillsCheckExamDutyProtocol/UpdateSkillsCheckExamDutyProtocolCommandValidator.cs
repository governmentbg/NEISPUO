namespace SB.Domain;

using FluentValidation;

public class UpdateSkillsCheckExamDutyProtocolCommandValidator : AbstractValidator<CreateSkillsCheckExamDutyProtocolCommand>
{
    public UpdateSkillsCheckExamDutyProtocolCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ProtocolNumber).MaximumLength(100);
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.SubjectId).NotNull();
        this.RuleFor(c => c.SubjectTypeId).NotNull();
        this.RuleFor(c => c.Date).NotEmpty();
        this.RuleFor(c => c.DirectorPersonId).NotEmpty();
        this.RuleFor(c => c.StudentsCapacity).NotEmpty();
        this.RuleFor(c => c.SupervisorPersonIds).NotEmpty();
    }
}
