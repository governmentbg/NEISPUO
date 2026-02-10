namespace SB.Domain;

using FluentValidation;

public class CreateSpbsBookRecordCommandValidator : AbstractValidator<CreateSpbsBookRecordCommand>
{
    public CreateSpbsBookRecordCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleFor(c => c.ClassId).NotEmpty();
        this.RuleFor(c => c.PersonId).NotEmpty();
    }
}
