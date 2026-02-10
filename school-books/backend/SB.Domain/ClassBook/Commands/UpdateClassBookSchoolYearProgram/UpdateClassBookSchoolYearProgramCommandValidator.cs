namespace SB.Domain;

using FluentValidation;

public class UpdateClassBookSchoolYearProgramCommandValidator : AbstractValidator<UpdateClassBookSchoolYearProgramCommand>
{
    public UpdateClassBookSchoolYearProgramCommandValidator()
    {
        this.RuleFor(s => s.SchoolYear).NotNull();
        this.RuleFor(s => s.InstId).NotNull();
        this.RuleFor(s => s.ClassBookId).NotNull();
        this.RuleFor(s => s.SysUserId).NotNull();
        this.RuleFor(s => s.SchoolYearProgram).MaximumLength(10000);
    }
}
