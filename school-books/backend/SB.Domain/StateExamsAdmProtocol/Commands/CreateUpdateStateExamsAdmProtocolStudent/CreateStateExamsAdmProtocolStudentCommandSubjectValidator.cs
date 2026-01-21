namespace SB.Domain;

using FluentValidation;

public class CreateStateExamsAdmProtocolStudentCommandSubjectValidator : AbstractValidator<CreateStateExamsAdmProtocolStudentCommandSubject>
{
    public CreateStateExamsAdmProtocolStudentCommandSubjectValidator()
    {
        this.RuleFor(c => c.SubjectId).NotNull();
        this.RuleFor(c => c.SubjectTypeId).NotNull();
    }
}
