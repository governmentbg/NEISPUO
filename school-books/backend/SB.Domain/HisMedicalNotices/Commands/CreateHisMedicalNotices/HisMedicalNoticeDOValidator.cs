namespace SB.Domain;

using FluentValidation;

public class HisMedicalNoticeDOValidator : AbstractValidator<HisMedicalNoticeDO>
{
    public HisMedicalNoticeDOValidator()
    {
        this.RuleFor(mn => mn.NrnMedicalNotice).NotEmpty().Length(12);
        this.RuleFor(mn => mn.NrnExamination).NotEmpty().Length(12);
        this.RuleFor(mn => mn.Patient).NotNull().SetValidator(new HisMedicalNoticePatientDOValidator());
        this.RuleFor(mn => mn.Practitioner).NotNull().SetValidator(new HisMedicalNoticePractitionerDOValidator());
        this.RuleFor(mn => mn.MedicalNotice).NotNull().SetValidator(new HisMedicalNoticeInfoDOValidator());
    }
}
