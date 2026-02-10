namespace SB.Domain;

using FluentValidation;

public class RemoveSchoolYearSettingsCommandValidator : AbstractValidator<RemoveSchoolYearSettingsCommand>
{
    public RemoveSchoolYearSettingsCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.SchoolYearSettingsId).NotNull();
    }
}
