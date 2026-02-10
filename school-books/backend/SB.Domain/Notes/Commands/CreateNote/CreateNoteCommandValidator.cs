namespace SB.Domain;

using FluentValidation;

public class CreateNoteCommandValidator : AbstractValidator<CreateNoteCommand>
{
    public CreateNoteCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Description).NotEmpty().MaximumLength(10000);
        this.RuleFor(c => c.IsForAllStudents).NotNull();
        this.RuleFor(c => c.StudentIds).NotNull();
        this.When(c => c.IsForAllStudents == true, () => {
            this.RuleFor(c => c.StudentIds).Empty();
        });
    }
}
