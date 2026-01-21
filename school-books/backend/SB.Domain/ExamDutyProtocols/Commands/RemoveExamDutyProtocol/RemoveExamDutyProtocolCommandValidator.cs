namespace SB.Domain;

using FluentValidation;

public class RemoveExamDutyProtocolCommandValidator : AbstractValidator<RemoveExamDutyProtocolCommand>
{
    public RemoveExamDutyProtocolCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ExamDutyProtocolId).NotNull();
    }
}
