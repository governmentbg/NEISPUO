namespace SB.Domain;
public partial interface IClassBookPrintRepository
{
    public record GetPgResultsVO(
        string FullName,
        string? CurriculumName,
        string? StartSchoolYearResult,
        string? EndSchoolYearResult);
}
