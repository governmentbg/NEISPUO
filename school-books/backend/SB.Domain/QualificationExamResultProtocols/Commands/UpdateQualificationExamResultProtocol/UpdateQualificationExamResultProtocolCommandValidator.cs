namespace SB.Domain;

using FluentValidation;

public class UpdateQualificationExamResultProtocolCommandValidator : AbstractValidator<CreateQualificationExamResultProtocolCommand>
{
    public UpdateQualificationExamResultProtocolCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ProtocolNumber).MaximumLength(100);
        this.RuleFor(c => c.SessionType).MaximumLength(100);
        this.RuleFor(c => c.GroupNum).MaximumLength(100);
        this.RuleFor(c => c.Profession).NotEmpty().MaximumLength(100);
        this.RuleFor(c => c.Speciality).NotEmpty().MaximumLength(100);
        this.RuleFor(c => c.QualificationDegreeId).NotNull();
        this.RuleFor(c => c.QualificationExamTypeId).NotNull();
        this.RuleFor(c => c.Date).NotEmpty();
        this.RuleFor(c => c.CommissionNominationOrderNumber).NotEmpty().MaximumLength(100);
        this.RuleFor(c => c.CommissionNominationOrderDate).NotNull();
        this.RuleFor(c => c.ClassIds).NotEmpty();
        this.RuleFor(c => c.CommissionChairman).NotNull();
        this.RuleFor(c => c.CommissionMembers).NotEmpty();
    }
}
