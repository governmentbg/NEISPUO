namespace SB.Domain;

using FluentValidation;

public class UpdateIndividualWorkCommandValidator : AbstractValidator<UpdateIndividualWorkCommand>
{
    public UpdateIndividualWorkCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.IndividualWorkId).NotNull();
        this.RuleFor(c => c.Date).NotNull();
        this.RuleFor(c => c.IndividualWorkActivity).NotEmpty().MaximumLength(10000);
    }
}
