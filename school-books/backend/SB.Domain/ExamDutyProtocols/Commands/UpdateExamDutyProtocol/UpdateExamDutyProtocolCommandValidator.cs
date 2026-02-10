namespace SB.Domain;

using FluentValidation;

public class UpdateExamDutyProtocolCommandValidator : AbstractValidator<CreateExamDutyProtocolCommand>
{
    public UpdateExamDutyProtocolCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ProtocolNumber).MaximumLength(100);
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.SessionType).MaximumLength(100);
        this.RuleFor(c => c.OrderNumber).MaximumLength(100);
        this.RuleFor(c => c.GroupNum).MaximumLength(100);
    }
}
