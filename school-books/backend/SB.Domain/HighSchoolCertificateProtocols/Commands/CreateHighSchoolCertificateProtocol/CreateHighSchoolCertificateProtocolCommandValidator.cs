namespace SB.Domain;

using FluentValidation;

public class CreateHighSchoolCertificateProtocolCommandValidator : AbstractValidator<CreateHighSchoolCertificateProtocolCommand>
{
    public CreateHighSchoolCertificateProtocolCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Stage).NotEmpty().IsInEnum();
        this.RuleFor(c => c.ProtocolNum).MaximumLength(100);
        this.RuleFor(c => c.CommissionMeetingDate).NotNull();
        this.RuleFor(c => c.CommissionNominationOrderNumber).NotEmpty().MaximumLength(100);
        this.RuleFor(c => c.CommissionNominationOrderDate).NotNull();
        this.RuleFor(c => c.ExamSession).MaximumLength(100);
        this.RuleFor(c => c.DirectorPersonId).NotNull();
        this.RuleFor(c => c.CommissionChairman).NotNull();
        this.RuleFor(c => c.CommissionMembers).NotEmpty();
    }
}
