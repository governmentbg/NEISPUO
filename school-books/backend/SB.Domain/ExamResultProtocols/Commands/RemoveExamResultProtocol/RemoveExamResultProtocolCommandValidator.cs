namespace SB.Domain;

using FluentValidation;

public class RemoveExamResultProtocolCommandValidator : AbstractValidator<RemoveExamResultProtocolCommand>
{
    public RemoveExamResultProtocolCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ExamResultProtocolId).NotNull();
    }
}
