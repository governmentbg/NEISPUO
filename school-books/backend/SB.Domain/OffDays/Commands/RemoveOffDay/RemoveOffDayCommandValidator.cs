namespace SB.Domain;

using FluentValidation;

public class RemoveOffDayCommandValidator : AbstractValidator<RemoveOffDayCommand>
{
    public RemoveOffDayCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.OffDayId).NotNull();
    }
}
