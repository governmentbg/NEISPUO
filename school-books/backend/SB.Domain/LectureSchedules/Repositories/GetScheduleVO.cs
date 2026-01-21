namespace SB.Domain;

using System;

public partial interface ILectureSchedulesQueryRepository
{
    public record GetScheduleVO(
        GetScheduleVOShiftHour[] ShiftHours,
        GetScheduleVOSlot[] Slots);

    public record GetScheduleVOShiftHour(
        int SlotNumber,
        string StartTime,
        string EndTime);

    public record GetScheduleVOSlot(
        int Day,
        int SlotNumber,
        GetScheduleVOSlotHour[] Hours);

    public record GetScheduleVOSlotHour(
        int ScheduleLessonId,
        int ClassBookId,
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
        int? LectureScheduleId,
        bool IsReplTeacher,
        bool? ReplTeacherIsNonSpecialist);
}
