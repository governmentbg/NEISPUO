namespace SB.Domain;

using FluentValidation;

public class UpdateReplrParticipationCommandValidator : AbstractValidator<UpdateReplrParticipationCommand>
{
    public UpdateReplrParticipationCommandValidator()
    {
        this.RuleFor(c => c.ReplrParticipationId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleFor(c => c.ReplrParticipationTypeId).NotNull();
        this.RuleFor(c => c.Date).NotNull();
        this.RuleFor(c => c.Attendees).NotEmpty().MaximumLength(10000);
        this.RuleFor(c => c.Topic).MaximumLength(10000);
    }
}
