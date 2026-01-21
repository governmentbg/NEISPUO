namespace SB.Domain;

using System.Text.Json.Serialization;

public partial interface IGradesQueryRepository
{
    public record GetCurriculumVO(
        int CurriculumId,
        string SubjectName,
        string? SubjectNameShort,
        string? CurriculumGroupName,
        [property: JsonIgnore] int SubjectTypeId,
        string SubjectTypeName)
    {
        public bool SubjectTypeIsProfilingSubject => Grade.SubjectTypeIsProfilingSubject(this.SubjectTypeId);

        public bool HasCreateGradeWithoutScheduleLessonAccess { get; set; } // should be mutable
    }
}
