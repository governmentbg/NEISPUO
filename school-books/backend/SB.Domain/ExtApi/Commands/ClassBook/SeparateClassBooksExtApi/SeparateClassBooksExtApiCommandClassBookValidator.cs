namespace SB.Domain;

using FluentValidation;

public class SeparateClassBooksExtApiCommandClassBookValidator : AbstractValidator<SeparateClassBooksExtApiCommandClassBook>
{
    public SeparateClassBooksExtApiCommandClassBookValidator()
    {
        this.RuleFor(s => s.ClassId).NotNull();
        this.RuleFor(c => c.ClassBookName).MaximumLength(300);
    }
}
