namespace SB.Domain;

using FluentValidation;

public class CreateIndividualWorkCommandValidator : AbstractValidator<CreateIndividualWorkCommand>
{
    public CreateIndividualWorkCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.PersonId).NotNull();
        this.RuleFor(c => c.Date).NotNull();
        this.RuleFor(c => c.IndividualWorkActivity).NotEmpty().MaximumLength(10000);
    }
}
