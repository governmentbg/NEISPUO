namespace SB.Domain;

using FluentValidation;

public class UpdatePgResultCommandValidator : AbstractValidator<UpdatePgResultCommand>
{
    public UpdatePgResultCommandValidator()
    {
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.PgResultId).NotNull();
        this.RuleFor(c => c.StartSchoolYearResult).MaximumLength(10000);
        this.RuleFor(c => c.EndSchoolYearResult).MaximumLength(10000);
        this.RuleFor(c => c)
            .Must(c =>
                !string.IsNullOrWhiteSpace(c.StartSchoolYearResult) ||
                !string.IsNullOrWhiteSpace(c.EndSchoolYearResult))
            .WithMessage("At least one of StartSchoolYearResult and EndSchoolYearResult must not be empty");
    }
}
