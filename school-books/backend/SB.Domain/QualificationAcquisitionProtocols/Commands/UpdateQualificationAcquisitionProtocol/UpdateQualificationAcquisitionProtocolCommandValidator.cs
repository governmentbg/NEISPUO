namespace SB.Domain;

using FluentValidation;

public class UpdateQualificationAcquisitionProtocolCommandValidator : AbstractValidator<CreateQualificationAcquisitionProtocolCommand>
{
    public UpdateQualificationAcquisitionProtocolCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ProtocolNumber).MaximumLength(100);
        this.RuleFor(c => c.Profession).NotEmpty().MaximumLength(100);
        this.RuleFor(c => c.Speciality).NotEmpty().MaximumLength(100);
        this.RuleFor(c => c.QualificationDegreeId).NotNull();
        this.RuleFor(c => c.Date).NotEmpty();
        this.RuleFor(c => c.CommissionNominationOrderNumber).NotEmpty().MaximumLength(100);
        this.RuleFor(c => c.CommissionNominationOrderDate).NotNull();
        this.RuleFor(c => c.DirectorPersonId).NotNull();
        this.RuleFor(c => c.CommissionChairman).NotNull();
        this.RuleFor(c => c.CommissionMembers).NotEmpty();
    }
}
