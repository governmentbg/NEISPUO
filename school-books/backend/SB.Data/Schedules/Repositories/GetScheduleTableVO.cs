namespace SB.Data;

using SB.Domain;
using System;

internal static partial class SchedulesQueryHelper
{
    public record GetScheduleTableVO(
        GetScheduleTableVOShiftHour[] ShiftHours,
        GetScheduleTableVOSlot[] Slots);

    public record GetScheduleTableVOShiftHour(
        int SlotNumber,
        string StartTime,
        string EndTime);

    public record GetScheduleTableVOSlot(
        int Day,
        int SlotNumber,
        GetScheduleTableVOSlotHour[] Hours);

    public record GetScheduleTableVOSlotHour(
        ClassBookType BookType,
        int? BasicClassId,
        int ClassBookId,
        int ScheduleLessonId,
        string ClassBookFullName,
        bool IsClassBookValid,
        DateTime Date,
        int CurriculumId,
        string? CurriculumGroupName,
        string SubjectName,
        string? SubjectNameShort,
        string SubjectTypeName,
        bool IsIndividualLesson,
        bool IsIndividualCurriculum,
        bool IsIndividualSchedule,
        int? StudentPersonId,
        string? StudentFirstName,
        string? StudentMiddleName,
        string? StudentLastName,
        GetScheduleTableVOSlotHourTeacher[] CurriculumTeachers,
        int? TeacherAbsenceId,
        int? LectureScheduleId,
        GetScheduleTableVOSlotHourTeacher? ReplTeacher,
        bool? ReplTeacherIsNonSpecialist,
        string? ExtTeacherName,
        bool? IsEmptyHour,
        bool? IsInUse,
        string? Location);

    public record GetScheduleTableVOSlotHourTeacher(
        int TeacherPersonId,
        string TeacherFirstName,
        string TeacherLastName,
        bool ActiveAtLessonTime,
        bool MarkedAsNoReplacement);
}
