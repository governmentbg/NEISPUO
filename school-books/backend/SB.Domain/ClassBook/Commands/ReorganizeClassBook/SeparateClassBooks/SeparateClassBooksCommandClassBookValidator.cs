namespace SB.Domain;

using FluentValidation;

public class SeparateClassBooksCommandClassBookValidator : AbstractValidator<SeparateClassBooksCommandClassBook>
{
    public SeparateClassBooksCommandClassBookValidator()
    {
        this.RuleFor(s => s.ClassId).NotNull();
        this.RuleFor(c => c.ClassBookName).MaximumLength(300);
    }
}
