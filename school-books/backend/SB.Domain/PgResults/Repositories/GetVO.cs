namespace SB.Domain;

public partial interface IPgResultsQueryRepository
{
    public record GetVO(
        int PgResultId,
        int PersonId,
        int? SubjectId,
        string? StartSchoolYearResult,
        string? EndSchoolYearResult);
}
