namespace SB.Domain;

using FluentValidation;

public class UpdateExamExtApiCommandValidator : AbstractValidator<UpdateExamExtApiCommand>
{
    public UpdateExamExtApiCommandValidator()
    {
        this.RuleFor(c => c.ExamId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Type).NotEmpty().IsInEnum();
        this.RuleFor(c => c.CurriculumId).NotNull();
        this.RuleFor(c => c.Date).NotEmpty();
        this.RuleFor(c => c.Description).MaximumLength(1000);
    }
}
