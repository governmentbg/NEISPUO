namespace SB.Domain;

using FluentValidation;

public class RemoveExamResultProtocolStudentCommandValidator : AbstractValidator<RemoveExamResultProtocolStudentCommand>
{
    public RemoveExamResultProtocolStudentCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ExamResultProtocolId).NotNull();
        this.RuleFor(c => c.ClassId).NotNull();
        this.RuleFor(c => c.PersonId).NotNull();
    }
}
