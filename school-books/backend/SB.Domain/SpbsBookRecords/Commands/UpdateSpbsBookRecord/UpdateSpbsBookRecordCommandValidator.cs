namespace SB.Domain;

using FluentValidation;

public class UpdateSpbsBookRecordCommandValidator : AbstractValidator<UpdateSpbsBookRecordCommand>
{
    public UpdateSpbsBookRecordCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.SpbsBookRecordId).NotNull();
        this.RuleFor(c => c.SendingCommission).MaximumLength(100);
        this.RuleFor(c => c.SendingCommissionAddress).MaximumLength(1000);
        this.RuleFor(c => c.SendingCommissionPhoneNumber).MaximumLength(100);
        this.RuleFor(c => c.InspectorNames).MaximumLength(100);
        this.RuleFor(c => c.InspectorAddress).MaximumLength(1000);
        this.RuleFor(c => c.InspectorPhoneNumber).MaximumLength(100);

        // movement
        this.RuleFor(c => c.CourtDecisionNumber).MaximumLength(100);
        this.RuleFor(c => c.IncommingLetterNumber).MaximumLength(100);
        this.RuleFor(c => c.IncommingDocNumber).MaximumLength(100);
        this.RuleFor(c => c.TransferReason).MaximumLength(1000);
        this.RuleFor(c => c.TransferProtocolNumber).MaximumLength(100);
        this.RuleFor(c => c.TransferLetterNumber).MaximumLength(100);
        this.RuleFor(c => c.TransferCertificateNumber).MaximumLength(100);
        this.RuleFor(c => c.TransferMessageNumber).MaximumLength(100);
    }
}
