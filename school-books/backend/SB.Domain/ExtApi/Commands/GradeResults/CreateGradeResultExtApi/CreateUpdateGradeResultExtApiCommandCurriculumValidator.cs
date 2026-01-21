namespace SB.Domain;

using FluentValidation;

public class CreateUpdateGradeResultExtApiCommandCurriculumValidator : AbstractValidator<CreateUpdateGradeResultExtApiCommandCurriculum>
{
    public CreateUpdateGradeResultExtApiCommandCurriculumValidator()
    {
        this.RuleFor(c => c.CurriculumId).NotNull();
    }
}
