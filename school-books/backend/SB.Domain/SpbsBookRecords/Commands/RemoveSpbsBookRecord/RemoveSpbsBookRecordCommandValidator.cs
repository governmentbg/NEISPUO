namespace SB.Domain;

using FluentValidation;

public class RemoveSpbsBookRecordCommandValidator : AbstractValidator<RemoveSpbsBookRecordCommand>
{
    public RemoveSpbsBookRecordCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.SpbsBookRecordId).NotNull();
    }
}
