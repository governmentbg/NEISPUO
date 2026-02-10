namespace SB.Domain;

using FluentValidation;

public class CreateExamDutyProtocolCommandValidator : AbstractValidator<CreateExamDutyProtocolCommand>
{
    public CreateExamDutyProtocolCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ProtocolNumber).MaximumLength(100);
        this.RuleFor(c => c.SubjectId).NotNull();
        this.RuleFor(c => c.SubjectTypeId).NotNull();
        this.RuleFor(c => c.ProtocolExamTypeId).NotNull();
        this.RuleFor(c => c.ProtocolExamSubTypeId).NotNull();
        this.RuleFor(c => c.OrderNumber).NotEmpty().MaximumLength(100);
        this.RuleFor(c => c.OrderDate).NotNull();
        this.RuleFor(c => c.ClassIds).NotEmpty();
    }
}
