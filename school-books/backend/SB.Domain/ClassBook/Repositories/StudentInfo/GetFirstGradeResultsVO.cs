namespace SB.Domain;

public partial interface IStudentInfoClassBooksQueryRepository
{
    public record GetFirstGradeResultsVO(
        QualitativeGrade? QualitativeGrade,
        SpecialNeedsGrade? SpecialGrade);
}
