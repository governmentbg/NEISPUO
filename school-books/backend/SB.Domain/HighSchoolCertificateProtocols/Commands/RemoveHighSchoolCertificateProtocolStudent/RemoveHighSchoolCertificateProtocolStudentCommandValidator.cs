namespace SB.Domain;

using FluentValidation;

public class RemoveHighSchoolCertificateProtocolStudentCommandValidator : AbstractValidator<RemoveHighSchoolCertificateProtocolStudentCommand>
{
    public RemoveHighSchoolCertificateProtocolStudentCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.HighSchoolCertificateProtocolId).NotNull();

        this.RuleFor(c => c.ClassId).NotNull();
        this.RuleFor(c => c.PersonId).NotNull();
    }
}
