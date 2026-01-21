namespace SB.Domain;

using FluentValidation;

public class CreateRemarkExtApiCommandValidator : AbstractValidator<CreateRemarkCommand>
{
    public CreateRemarkExtApiCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.PersonId).NotNull();
        this.RuleFor(c => c.CurriculumId).NotNull();
        this.RuleFor(c => c.Type).NotEmpty().IsInEnum();
        this.RuleFor(c => c.Date).NotEmpty();
        this.RuleFor(c => c.Description).NotEmpty().MaximumLength(1000);
    }
}
