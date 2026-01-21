namespace SB.Domain;

public partial interface IGradesQueryRepository
{
    public record GetCurriculumStudentsVO(
        int PersonId,
        int? ClassNumber,
        string FirstName,
        string? MiddleName,
        string LastName,
        bool IsTransferred,
        bool NotEnrolledInCurriculum,
        bool WithoutFirstTermGrade,
        bool WithoutSecondTermGrade,
        bool WithoutFinalGrade,
        bool HasSpecialNeeds);
}
