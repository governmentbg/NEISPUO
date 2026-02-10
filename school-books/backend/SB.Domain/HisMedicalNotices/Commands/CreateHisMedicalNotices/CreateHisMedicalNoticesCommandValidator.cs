namespace SB.Domain;

using FluentValidation;

public class CreateHisMedicalNoticesCommandValidator : AbstractValidator<CreateHisMedicalNoticesCommand>
{
    public CreateHisMedicalNoticesCommandValidator()
    {
        this.RuleFor(c => c.MedicalNotices)
            .NotNull()
            .NotEmpty()
            .ForEach(mn => mn.NotNull().SetValidator(new HisMedicalNoticeDOValidator()));
    }
}
