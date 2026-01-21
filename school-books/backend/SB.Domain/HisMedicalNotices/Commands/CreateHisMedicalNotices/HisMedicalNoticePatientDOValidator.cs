namespace SB.Domain;

using FluentValidation;

public class HisMedicalNoticePatientDOValidator : AbstractValidator<HisMedicalNoticePatientDO>
{
    public HisMedicalNoticePatientDOValidator()
    {
        this.RuleFor(mn => mn.IdentifierType).InclusiveBetween(1, 5);
        this.RuleFor(mn => mn.Identifier).NotEmpty().MaximumLength(255);
        this.RuleFor(mn => mn.GivenName).NotNull().MaximumLength(255);
        this.RuleFor(mn => mn.FamilyName).NotNull().MaximumLength(255);
    }
}
