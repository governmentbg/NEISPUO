namespace SB.Domain;

using FluentValidation;

public class RemoveHighSchoolCertificateProtocolCommandValidator : AbstractValidator<RemoveHighSchoolCertificateProtocolCommand>
{
    public RemoveHighSchoolCertificateProtocolCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.HighSchoolCertificateProtocolId).NotNull();
    }
}
