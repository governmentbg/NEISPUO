namespace SB.Domain;

using FluentValidation;

public class HisMedicalNoticePractitionerDOValidator : AbstractValidator<HisMedicalNoticePractitionerDO>
{
    public HisMedicalNoticePractitionerDOValidator()
    {
        this.RuleFor(mn => mn.Pmi).NotEmpty().MaximumLength(255);
    }
}
