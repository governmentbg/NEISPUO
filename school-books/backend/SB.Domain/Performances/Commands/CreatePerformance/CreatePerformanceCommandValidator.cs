namespace SB.Domain;

using FluentValidation;

public class CreatePerformanceCommandValidator : AbstractValidator<CreatePerformanceCommand>
{
    public CreatePerformanceCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleFor(c => c.PerformanceTypeId).NotNull();
        this.RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
        this.RuleFor(c => c.Description).NotEmpty().MaximumLength(10000);
        this.RuleFor(c => c.StartDate).NotNull();
        this.RuleFor(c => c.EndDate).NotNull();
        this.RuleFor(c => c.EndDate).GreaterThanOrEqualTo(c => c.StartDate)
            .WithUserMessage("Начална дата трябва да е преди Крайна дата.");
        this.RuleFor(c => c.Location).NotEmpty().MaximumLength(100);
        this.RuleFor(c => c.StudentAwards).MaximumLength(10000);
    }
}
