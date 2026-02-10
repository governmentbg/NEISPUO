namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public partial interface IMyHourQueryRepository
{
    public record GetTeacherLessonsVO(
        int ScheduleLessonId,
        int HourNumber,
        [property: JsonIgnore] TimeSpan StartTimeInternal,
        [property: JsonIgnore] TimeSpan EndTimeInternal,
        int ClassBookId,
        string ClassBookFullName,
        bool IsIndividualSchedule,
        bool IsIndividualLesson,
        bool IsIndividualCurriculum,
        bool IsTaken,
        string StudentFirstName,
        string? StudentMiddleName,
        string StudentLastName,
        int CurriculumId,
        string? CurriculumGroupName,
        string SubjectName,
        string? SubjectNameShort,
        string SubjectTypeName,
        bool? ReplTeacherIsNonSpecialist)
    {
        public string StartTime => this.StartTimeInternal.ToString(@"hh\:mm");
        public string EndTime => this.EndTimeInternal.ToString(@"hh\:mm");
    }
}
