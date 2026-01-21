namespace SB.Domain;

using FluentValidation;

public class CreateClassBookCommandClassBookValidator : AbstractValidator<CreateClassBookCommandClassBook>
{
    public CreateClassBookCommandClassBookValidator()
    {
        this.RuleFor(s => s.ClassId).NotNull();
        this.RuleFor(c => c.ClassBookName).MaximumLength(300);
    }
}
