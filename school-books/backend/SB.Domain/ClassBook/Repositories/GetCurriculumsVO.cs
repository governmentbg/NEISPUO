namespace SB.Domain;

using System.Text.Json.Serialization;

public partial interface IClassBooksQueryRepository
{
    public record GetCurriculumsVO(
        int CurriculumId,
        int? ParentCurriculumId,
        string SubjectName,
        string? SubjectNameShort,
        string? CurriculumGroupName,
        bool IsIndividualLesson,
        bool IsIndividualCurriculum,
        [property: JsonIgnore] int SubjectTypeId,
        string SubjectTypeName,
        bool WithoutGrade,
        bool IsValid,
        [property: JsonIgnore] int[] WriteAccessCurriculumTeacherPersonIds,
        [property: JsonIgnore] int[] ReplTeacherPersonIds)
    {
        public bool SubjectTypeIsProfilingSubject => Grade.SubjectTypeIsProfilingSubject(this.SubjectTypeId);

        public bool HasCreateGradeAccess { get; set; } // should be mutable
        public bool HasCreateForecastGradeAccess { get; set; } // should be mutable
        public bool HasCreateTopicPlanAccess { get; set; } // should be mutable
    }
}
