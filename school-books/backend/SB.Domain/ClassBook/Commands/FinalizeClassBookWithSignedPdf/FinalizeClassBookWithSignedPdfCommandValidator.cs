namespace SB.Domain;

using FluentValidation;

public class FinalizeClassBookWithSignedPdfCommandValidator : AbstractValidator<FinalizeClassBookWithSignedPdfCommand>
{
    public FinalizeClassBookWithSignedPdfCommandValidator()
    {
        this.RuleFor(s => s.SchoolYear).NotNull();
        this.RuleFor(s => s.InstId).NotNull();
        this.When(c => c.ExtractClassBookIdFromMetadataOrFileName == false, () => {
            this.RuleFor(s => s.ClassBookId).NotNull();
        });
        this.RuleFor(s => s.SysUserId).NotNull();
        this.RuleFor(s => s.SignedClassBookPrintFile).NotNull();
        this.RuleFor(s => s.SignedClassBookPrintFileName).NotEmpty();
        this.RuleFor(s => s.ExtractClassBookIdFromMetadataOrFileName).NotNull();
    }
}
