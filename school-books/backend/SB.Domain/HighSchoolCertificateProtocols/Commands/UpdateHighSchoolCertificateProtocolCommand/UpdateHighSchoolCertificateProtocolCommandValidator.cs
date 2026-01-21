namespace SB.Domain;

using FluentValidation;

public class UpdateHighSchoolCertificateProtocolCommandValidator : AbstractValidator<UpdateHighSchoolCertificateProtocolCommand>
{
    public UpdateHighSchoolCertificateProtocolCommandValidator(IValidator<CreateHighSchoolCertificateProtocolCommand> createValidator)
    {
        this.RuleFor(s => (CreateHighSchoolCertificateProtocolCommand)s).SetValidator(createValidator);
        this.RuleFor(s => s.HighSchoolCertificateProtocolId).NotNull();
    }
}
