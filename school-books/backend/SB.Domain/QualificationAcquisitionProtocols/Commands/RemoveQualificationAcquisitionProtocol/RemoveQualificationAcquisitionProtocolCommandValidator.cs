namespace SB.Domain;

using FluentValidation;

public class RemoveQualificationAcquisitionProtocolCommandValidator : AbstractValidator<RemoveQualificationAcquisitionProtocolCommand>
{
    public RemoveQualificationAcquisitionProtocolCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.QualificationAcquisitionProtocolId).NotNull();
    }
}
