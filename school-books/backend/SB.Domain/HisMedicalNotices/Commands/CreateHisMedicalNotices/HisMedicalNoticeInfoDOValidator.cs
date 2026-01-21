namespace SB.Domain;

using FluentValidation;

public class HisMedicalNoticeInfoDOValidator : AbstractValidator<HisMedicalNoticeInfoDO>
{
    public HisMedicalNoticeInfoDOValidator()
    {
        this.RuleFor(mn => mn.FromDate).NotNull();
        this.RuleFor(mn => mn.ToDate).NotNull();
        this.RuleFor(mn => mn.AuthoredOn).NotNull();
    }
}
