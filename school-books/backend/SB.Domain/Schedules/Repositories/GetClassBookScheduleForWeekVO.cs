namespace SB.Domain;

using System;

public partial interface ISchedulesQueryRepository
{
    public record GetClassBookScheduleForWeekVO(
        GetClassBookScheduleForWeekVOHour[] Hours,
        int[] OffDays);

    public record GetClassBookScheduleForWeekVOHour(
        int ScheduleLessonId,
        DateTime Date,
        int Day,
        int HourNumber,
        int CurriculumId,
        string? CurriculumGroupName,
        string SubjectName,
        string? SubjectNameShort,
        string SubjectTypeName,
        bool IsIndividualLesson,
        bool IsIndividualCurriculum,
        GetClassBookScheduleForWeekVOHourTeacher[] CurriculumTeachers,
        int? TeacherAbsenceId,
        GetClassBookScheduleForWeekVOHourTeacher? ReplTeacher,
        bool? ReplTeacherIsNonSpecialist,
        string? ExtTeacherName,
        bool? IsEmptyHour)
    {
        public bool HasAbsenceCreateAccess { get; set; } // should be mutable

        public bool HasAbsenceExcuseAccess { get; set; } // should be mutable

        public bool HasAbsenceRemoveAccess { get; set; } // should be mutable

        public bool HasTopicCreateAccess { get; set; } // should be mutable
    }

    public record GetClassBookScheduleForWeekVOHourTeacher(
        int TeacherPersonId,
        string TeacherFirstName,
        string TeacherLastName,
        bool MarkedAsNoReplacement);
}
