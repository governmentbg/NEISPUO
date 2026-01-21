namespace SB.Domain;

using FluentValidation;

public class CreateGradeChangeExamsAdmProtocolStudentCommandSubjectValidator : AbstractValidator<CreateGradeChangeExamsAdmProtocolStudentCommandSubject>
{
    public CreateGradeChangeExamsAdmProtocolStudentCommandSubjectValidator()
    {
        this.RuleFor(c => c.SubjectId).NotNull();
        this.RuleFor(c => c.SubjectTypeId).NotNull();
    }
}
