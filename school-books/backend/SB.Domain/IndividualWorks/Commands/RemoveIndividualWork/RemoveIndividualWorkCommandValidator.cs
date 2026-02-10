namespace SB.Domain;

using FluentValidation;

public class RemoveIndividualWorkCommandValidator : AbstractValidator<RemoveIndividualWorkCommand>
{
    public RemoveIndividualWorkCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.IndividualWorkId).NotNull();
    }
}
