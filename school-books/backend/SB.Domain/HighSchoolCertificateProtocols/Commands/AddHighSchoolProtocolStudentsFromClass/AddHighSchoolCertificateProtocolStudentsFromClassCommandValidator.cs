namespace SB.Domain;

using FluentValidation;

public class AddHighSchoolCertificateProtocolStudentsFromClassCommandValidator : AbstractValidator<AddHighSchoolCertificateProtocolStudentsFromClassCommand>
{
    public AddHighSchoolCertificateProtocolStudentsFromClassCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ClassId).NotNull();
    }
}
