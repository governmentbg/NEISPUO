namespace SB.Domain;

using FluentValidation;

public class UpdateRemarkExtApiCommandValidator : AbstractValidator<UpdateRemarkExtApiCommand>
{
    public UpdateRemarkExtApiCommandValidator()
    {
        this.RuleFor(c => c.RemarkId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Date).NotEmpty();
        this.RuleFor(c => c.Description).NotEmpty().MaximumLength(1000);
    }
}
