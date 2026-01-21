namespace SB.Domain;

using FluentValidation;

public class CreatePublicationCommandFileValidator : AbstractValidator<CreatePublicationCommandFile>
{
    public CreatePublicationCommandFileValidator()
    {
        this.RuleFor(c => c.BlobId).NotNull();
        this.RuleFor(c => c.FileName).NotEmpty().MaximumLength(500);
    }
}
