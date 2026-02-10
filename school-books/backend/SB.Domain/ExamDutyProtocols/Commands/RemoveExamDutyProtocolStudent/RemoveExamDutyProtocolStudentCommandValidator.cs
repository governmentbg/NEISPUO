namespace SB.Domain;

using FluentValidation;

public class RemoveExamDutyProtocolStudentCommandValidator : AbstractValidator<RemoveExamDutyProtocolStudentCommand>
{
    public RemoveExamDutyProtocolStudentCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ExamDutyProtocolId).NotNull();
        this.RuleFor(c => c.ClassId).NotNull();
        this.RuleFor(c => c.PersonId).NotNull();
    }
}
