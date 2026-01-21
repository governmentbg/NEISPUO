namespace SB.Domain;

public partial interface IClassBooksAdminQueryRepository
{
    public record GetCurriculumVO(
        int CurriculumId,
        string SubjectName,
        string SubjectTypeName,
        string? CurriculumGroupName,
        bool IsIndividualLesson,
        bool IsIndividualCurriculum,
        string TeacherNames,
        bool WithoutGrade);
}
