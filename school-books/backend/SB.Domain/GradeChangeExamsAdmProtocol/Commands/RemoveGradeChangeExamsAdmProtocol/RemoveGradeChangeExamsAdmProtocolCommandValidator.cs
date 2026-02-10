namespace SB.Domain;

using FluentValidation;

public class RemoveGradeChangeExamsAdmProtocolCommandValidator: AbstractValidator<RemoveGradeChangeExamsAdmProtocolCommand>
{
    public RemoveGradeChangeExamsAdmProtocolCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.GradeChangeExamsAdmProtocolId).NotNull();
    }
}
