namespace SB.Domain;

public partial interface IClassBookPrintRepository
{
    public record GetGradeResultsVO(
        int? ClassNumber,
        string FullName,
        bool IsTransferred,
        GradeResultType? InitialResult,
        string[] RetakeSubjectNames
    );
}
