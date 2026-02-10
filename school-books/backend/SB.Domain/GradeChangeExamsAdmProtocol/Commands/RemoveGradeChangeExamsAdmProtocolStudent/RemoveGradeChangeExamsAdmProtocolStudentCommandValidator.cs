namespace SB.Domain;

using FluentValidation;

public class RemoveGradeChangeExamsAdmProtocolStudentCommandValidator : AbstractValidator<RemoveGradeChangeExamsAdmProtocolStudentCommand>
{
    public RemoveGradeChangeExamsAdmProtocolStudentCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleFor(c => c.ClassId).NotNull();
        this.RuleFor(c => c.PersonId).NotNull();
    }
}
