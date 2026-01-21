namespace SB.Domain;

using FluentValidation;

public class UpdateGraduationThesisDefenseProtocolCommandValidator : AbstractValidator<CreateGraduationThesisDefenseProtocolCommand>
{
    public UpdateGraduationThesisDefenseProtocolCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ProtocolNumber).MaximumLength(100);
        this.RuleFor(c => c.SessionType).MaximumLength(100);
        this.RuleFor(c => c.CommissionMeetingDate).NotEmpty();
        this.RuleFor(c => c.DirectorOrderNumber).NotEmpty().MaximumLength(100);
        this.RuleFor(c => c.DirectorOrderDate).NotNull();
        this.RuleFor(c => c.CommissionChairman).NotNull();
        this.RuleFor(c => c.CommissionMembers).NotEmpty();
        this.RuleFor(c => c.Section1StudentsCapacity).NotNull();
        this.RuleFor(c => c.Section2StudentsCapacity).NotNull();
        this.RuleFor(c => c.Section3StudentsCapacity).NotNull();
        this.RuleFor(c => c.Section4StudentsCapacity).NotNull();
    }
}
