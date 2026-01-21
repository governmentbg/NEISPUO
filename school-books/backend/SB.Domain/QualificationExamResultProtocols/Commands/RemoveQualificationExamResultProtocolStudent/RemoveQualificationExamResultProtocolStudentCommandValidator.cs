namespace SB.Domain;

using FluentValidation;

public class RemoveQualificationExamResultProtocolStudentCommandValidator : AbstractValidator<RemoveQualificationExamResultProtocolStudentCommand>
{
    public RemoveQualificationExamResultProtocolStudentCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.QualificationExamResultProtocolId).NotNull();
        this.RuleFor(c => c.ClassId).NotNull();
        this.RuleFor(c => c.PersonId).NotNull();
    }
}
