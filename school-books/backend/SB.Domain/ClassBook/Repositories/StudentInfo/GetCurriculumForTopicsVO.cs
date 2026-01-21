namespace SB.Domain;

public partial interface IStudentInfoClassBooksQueryRepository
{
    public record GetCurriculumForTopicsVO(
        int CurriculumId,
        int? ParentCurriculumId,
        string SubjectName,
        string? SubjectNameShort,
        string? CurriculumGroupName,
        string SubjectTypeName);
}
