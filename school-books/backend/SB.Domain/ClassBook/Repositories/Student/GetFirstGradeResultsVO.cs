namespace SB.Domain;

public partial interface IStudentClassBooksQueryRepository
{
    public record GetFirstGradeResultsVO(
        QualitativeGrade? QualitativeGrade,
        SpecialNeedsGrade? SpecialGrade);
}
