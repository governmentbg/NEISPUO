namespace SB.Domain;

using FluentValidation;

public class RemoveQualificationAcquisitionProtocolStudentCommandValidator : AbstractValidator<RemoveQualificationAcquisitionProtocolStudentCommand>
{
    public RemoveQualificationAcquisitionProtocolStudentCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.QualificationAcquisitionProtocolId).NotNull();
        this.RuleFor(c => c.ClassId).NotNull();
        this.RuleFor(c => c.PersonId).NotNull();
    }
}
