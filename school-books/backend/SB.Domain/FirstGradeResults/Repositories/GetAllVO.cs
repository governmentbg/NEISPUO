namespace SB.Domain;
public partial interface IFirstGradeResultsQueryRepository
{
    public record GetAllVO(
        int PersonId,
        QualitativeGrade? QualitativeGrade,
        SpecialNeedsGrade? SpecialGrade);
}
