namespace SB.Domain;

public partial interface IGradeResultsQueryRepository
{
    public record GetAllEditVO(
        int PersonId,
        GradeResultType InitialResultType,
        int[] RetakeExamCurriculumIds,
        GradeResultType? FinalResultType);
}
