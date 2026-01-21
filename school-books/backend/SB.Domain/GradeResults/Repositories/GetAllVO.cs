namespace SB.Domain;

public partial interface IGradeResultsQueryRepository
{
    public record GetAllVO(
        int PersonId,
        GradeResultType InitialResultType,
        string? RetakeExamSubjects,
        GradeResultType? FinalResultType);
}
