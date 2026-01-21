namespace SB.Domain;

using FluentValidation;

public class UpdateGradeResultSessionsCommandSessionValidator : AbstractValidator<UpdateGradeResultSessionsCommandSession>
{
    public UpdateGradeResultSessionsCommandSessionValidator()
    {
        this.RuleFor(c => c.PersonId).NotNull();
        this.RuleFor(c => c.CurriculumId).NotNull();
    }
}
