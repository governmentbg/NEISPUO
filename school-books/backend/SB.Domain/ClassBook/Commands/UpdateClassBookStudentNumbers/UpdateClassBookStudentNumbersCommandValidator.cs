namespace SB.Domain;

using FluentValidation;

public class UpdateClassBookStudentNumbersCommandValidator : AbstractValidator<UpdateClassBookStudentNumbersCommand>
{
    public UpdateClassBookStudentNumbersCommandValidator(IValidator<UpdateClassBookStudentNumbersCommandStudent> studentValidator)
    {
        this.RuleFor(s => s.SchoolYear).NotNull();
        this.RuleFor(s => s.InstId).NotNull();
        this.RuleFor(s => s.ClassBookId).NotNull();
        this.RuleFor(s => s.SysUserId).NotNull();

        this.RuleFor(c => c.Students).NotNull();
        this.RuleForEach(c => c.Students).SetValidator(studentValidator);
    }
}
