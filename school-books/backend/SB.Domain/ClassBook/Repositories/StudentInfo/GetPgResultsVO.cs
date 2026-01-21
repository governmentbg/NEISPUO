namespace SB.Domain;

public partial interface IStudentInfoClassBooksQueryRepository
{
    public record GetPgResultsVO(
        int PgResultId,
        string? CurriculumName,
        string? StartSchoolYearResult,
        string? EndSchoolYearResult);
}
