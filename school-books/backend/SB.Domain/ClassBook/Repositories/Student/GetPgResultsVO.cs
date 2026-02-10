namespace SB.Domain;

public partial interface IStudentClassBooksQueryRepository
{
    public record GetPgResultsVO(
        int PgResultId,
        string? CurriculumName,
        string? StartSchoolYearResult,
        string? EndSchoolYearResult);
}
