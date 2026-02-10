namespace SB.Domain;

public partial interface IClassBookStudentPrintRepository
{
    public record GetStudentPgResultsVO(
        string? StartSchoolYearResult,
        string? EndSchoolYearResult);
}
