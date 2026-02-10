namespace SB.Domain;

using FluentValidation;

public class АcknowledMedicalNoticesCommandValidator : AbstractValidator<АcknowledMedicalNoticesCommand>
{
    public АcknowledMedicalNoticesCommandValidator()
    {
        this.RuleFor(c => c.HisMedicalNoticeIds).NotNull().NotEmpty();
    }
}
