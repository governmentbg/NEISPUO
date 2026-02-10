namespace SB.Domain;

using FluentValidation;

public class CreateSupportExtApiCommandValidator : AbstractValidator<CreateSupportExtApiCommand>
{
    public CreateSupportExtApiCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.EndDate).NotEmpty();
        this.RuleFor(c => c.StudentIds).NotNull();
        this.RuleFor(c => c.TeacherIds).NotEmpty();
        this.RuleFor(c => c.Description).MaximumLength(10000);
        this.RuleForEach(c => c.SupportDifficultyTypeIds).NotEmpty();
        this.RuleFor(c => c.ExpectedResult).MaximumLength(10000);
        this.RuleFor(c => c.IsForAllStudents).NotNull();
        this.RuleFor(c => c.StudentIds).NotNull();
        this.When(c => c.IsForAllStudents == true, () => {
            this.RuleFor(c => c.StudentIds).Empty();
        });
        this.When(c => c.IsForAllStudents == false, () => {
            this.RuleFor(c => c.StudentIds).NotEmpty();
        });

    }
}
