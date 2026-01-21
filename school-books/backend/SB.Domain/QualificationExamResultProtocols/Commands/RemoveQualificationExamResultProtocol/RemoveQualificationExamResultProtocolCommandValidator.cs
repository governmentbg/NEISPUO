namespace SB.Domain;

using FluentValidation;

public class RemoveQualificationExamResultProtocolCommandValidator : AbstractValidator<RemoveQualificationExamResultProtocolCommand>
{
    public RemoveQualificationExamResultProtocolCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.QualificationExamResultProtocolId).NotNull();
    }
}
