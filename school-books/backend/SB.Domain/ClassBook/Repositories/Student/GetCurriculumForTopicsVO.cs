namespace SB.Domain;

public partial interface IStudentClassBooksQueryRepository
{
    public record GetCurriculumForTopicsVO(
        int CurriculumId,
        string SubjectName,
        string? SubjectNameShort,
        string? CurriculumGroupName,
        string SubjectTypeName);
}
