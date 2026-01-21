namespace SB.Domain;

using FluentValidation;

public class CreateSpbsBookRecordAbsenceCommandValidator : AbstractValidator<CreateSpbsBookRecordAbsenceCommand>
{
    public CreateSpbsBookRecordAbsenceCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.SpbsBookRecordId).NotNull();

        this.RuleFor(c => c.AbsenceDate).NotEmpty();
        this.RuleFor(c => c.AbsenceReason).NotEmpty().MaximumLength(1000);
    }
}
