namespace SB.Data;

using SB.Domain;
using System;

internal static partial class SchedulesQueryHelper
{
    public record GetScheduleHoursVO
    {
        public ClassBookType BookType { get; init; }
        public int? BasicClassId { get; init; }
        public int ClassBookId { get; init; }
        public int ScheduleLessonId { get; init; }
        public required string ClassBookFullName { get; init; }
        public bool IsClassBookValid { get; init; }
        public DateTime Date { get; init; }
        public int Day { get; init; }
        public int HourNumber { get; init; }
        public int ShiftId { get; init; }
        public int CurriculumId { get; init; }
        public string? CurriculumGroupName { get; init; }
        public required string SubjectName { get; init; }
        public string? SubjectNameShort { get; init; }
        public required string SubjectTypeName { get; init; }
        public bool IsIndividualLesson { get; init; }
        public bool IsIndividualCurriculum { get; init; }
        public bool IsIndividualSchedule { get; init; }
        public int? StudentPersonId { get; init; }
        public string? StudentFirstName { get; init; }
        public string? StudentMiddleName { get; init; }
        public string? StudentLastName { get; init; }
        public required GetScheduleHoursVOTeacher[] CurriculumTeachers { get; init; }
        public int? TeacherAbsenceId { get; init; }
        public GetScheduleHoursVOTeacher? ReplTeacher { get; init; }
        public bool? ReplTeacherIsNonSpecialist { get; init; }
        public string? ExtTeacherName { get; init; }
        public bool? IsEmptyHour { get; init; }
        public bool? IsInUse { get; init; }
        public int? LectureScheduleId { get; init; }
        public string? Location { get; init; }
    }

    public record GetScheduleHoursVOTeacher
    {
        public int TeacherPersonId { get; init; }
        public required string TeacherFirstName { get; init; }
        public required string TeacherLastName { get; init; }
        public bool ActiveAtLessonTime { get; init; }
        public bool MarkedAsNoReplacement { get; init; }
    }
}
