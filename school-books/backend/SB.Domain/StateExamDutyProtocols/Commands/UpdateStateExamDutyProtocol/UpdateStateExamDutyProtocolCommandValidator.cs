namespace SB.Domain;

using FluentValidation;

public class UpdateStateExamDutyProtocolCommandValidator : AbstractValidator<CreateStateExamDutyProtocolCommand>
{
    public UpdateStateExamDutyProtocolCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ProtocolNumber).MaximumLength(100);
        this.RuleFor(c => c.SessionType).MaximumLength(100);
        this.RuleFor(c => c.SubjectId).NotNull();
        this.RuleFor(c => c.SubjectTypeId).NotNull();
        this.RuleFor(c => c.Date).NotEmpty();
        this.RuleFor(c => c.OrderNumber).NotEmpty().MaximumLength(100);
        this.RuleFor(c => c.OrderDate).NotNull();
        this.RuleFor(c => c.ModulesCount).NotNull();
        this.RuleFor(c => c.RoomNumber).MaximumLength(100);
        this.RuleFor(c => c.SupervisorPersonIds).NotEmpty();
    }
}
