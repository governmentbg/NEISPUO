namespace SB.Domain;

using FluentValidation;

public class CreatePublicationCommandValidator : AbstractValidator<CreatePublicationCommand>
{
    public CreatePublicationCommandValidator(IValidator<CreatePublicationCommandFile> fileValidator)
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleFor(c => c.Type).NotEmpty().IsInEnum();
        this.RuleFor(c => c.Date).NotEmpty();
        this.RuleFor(c => c.Title).NotEmpty().MaximumLength(100);
        this.RuleFor(c => c.Content).NotEmpty().MaximumLength(1000);
        this.RuleFor(c => c.Files).NotNull();
        this.RuleForEach(c => c.Files).SetValidator(fileValidator);
    }
}
