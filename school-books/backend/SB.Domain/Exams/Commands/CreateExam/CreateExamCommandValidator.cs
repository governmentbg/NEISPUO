namespace SB.Domain;

using FluentValidation;

public class CreateExamCommandValidator : AbstractValidator<CreateExamCommand>
{
    public CreateExamCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.CurriculumId).NotNull();
        this.RuleFor(c => c.Type).NotEmpty().IsInEnum();
        this.RuleFor(c => c.Date).NotEmpty();
        this.RuleFor(c => c.Description).MaximumLength(1000);
    }
}
