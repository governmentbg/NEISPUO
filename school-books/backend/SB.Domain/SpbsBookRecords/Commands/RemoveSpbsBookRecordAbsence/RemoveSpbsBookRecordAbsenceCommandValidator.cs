namespace SB.Domain;

using FluentValidation;

public class RemoveSpbsBookRecordAbsenceCommandValidator : AbstractValidator<RemoveSpbsBookRecordAbsenceCommand>
{
    public RemoveSpbsBookRecordAbsenceCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.SpbsBookRecordId).NotNull();
        this.RuleFor(c => c.OrderNum).NotNull();
    }
}
