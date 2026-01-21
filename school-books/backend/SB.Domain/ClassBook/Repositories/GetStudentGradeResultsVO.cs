namespace SB.Domain;

public partial interface IClassBookStudentPrintRepository
{
    public record GetStudentGradeResultsVO(
        GradeResultType? InitialResult,
        string[] RetakeSubjectNames
    );
}
