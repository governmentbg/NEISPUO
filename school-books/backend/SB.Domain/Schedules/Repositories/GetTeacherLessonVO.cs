namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public partial interface IMyHourQueryRepository
{
    public record GetTeacherLessonVO(
        int ClassBookId,
        int ScheduleLessonId,
        int HourNumber,
        [property: JsonIgnore] TimeSpan StartTimeInternal,
        [property: JsonIgnore] TimeSpan EndTimeInternal,
        int? TeacherAbsenceId,
        int CurriculumId,
        int? IndividualCurriculumPersonId,
        bool HasTopicPlan,
        int? TopicId,
        string[] TopicTitles,
        DateTime? TopicCreateDate,
        [property: JsonIgnore] int? TopicCreatedBySysUserId,
        bool IsVerified,
        GetTeacherLessonVOStudent[] Students)
        {
            public string StartTime => this.StartTimeInternal.ToString(@"hh\:mm");
            public string EndTime => this.EndTimeInternal.ToString(@"hh\:mm");
            public bool TopicHasUndoAccess { get; set; } // should be mutable
            public bool TopicHasRemoveAccess { get; set; } // should be mutable
        }

    public record GetTeacherLessonVOStudent(
        int PersonId,
        int? ClassNumber,
        string FirstName,
        string? MiddleName,
        string LastName,
        bool IsTransferred,
        bool NotEnrolledInCurriculum,
        bool WithoutFirstTermGrade,
        bool WithoutSecondTermGrade,
        bool HasSpecialNeeds);
}
