namespace SB.Domain;

using FluentValidation;

public class UpdateNoteCommandValidator : AbstractValidator<UpdateNoteCommand>
{
    public UpdateNoteCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.NoteId).NotNull();
        this.RuleFor(c => c.Description).NotEmpty().MaximumLength(10000);
        this.RuleFor(c => c.IsForAllStudents).NotNull();
        this.RuleFor(c => c.StudentIds).NotNull();
        this.When(c => c.IsForAllStudents == true, () => {
            this.RuleFor(c => c.StudentIds).Empty();
        });
    }
}
