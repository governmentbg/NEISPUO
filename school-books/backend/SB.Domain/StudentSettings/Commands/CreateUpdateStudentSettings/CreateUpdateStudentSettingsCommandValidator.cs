namespace SB.Domain;

using FluentValidation;

internal class CreateUpdateStudentSettingsCommandValidator : AbstractValidator<CreateUpdateStudentSettingsCommand>
{
    public CreateUpdateStudentSettingsCommandValidator()
    {
        this.RuleFor(c => c.AllowAbsenceEmails).NotNull();
        this.RuleFor(c => c.AllowGradeEmails).NotNull();
        this.RuleFor(c => c.AllowRemarkEmails).NotNull();
        this.RuleFor(c => c.AllowAbsenceNotifications).NotNull();
        this.RuleFor(c => c.AllowGradeNotifications).NotNull();
        this.RuleFor(c => c.AllowRemarkNotifications).NotNull();
    }
}
