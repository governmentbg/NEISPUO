namespace SB.Domain;

public partial interface ISchedulesQueryRepository
{
    public record GetTeacherScheduleTableForWeekVO(
        GetTeacherScheduleTableForWeekVOShiftHour[] ShiftHours,
        GetTeacherScheduleTableForWeekVOSlot[] Slots);

    public record GetTeacherScheduleTableForWeekVOShiftHour(
        int SlotNumber,
        string StartTime,
        string EndTime);

    public record GetTeacherScheduleTableForWeekVOSlot(
        int Day,
        int SlotNumber,
        GetTeacherScheduleTableForWeekVOSlotHour[] Hours);

    public record GetTeacherScheduleTableForWeekVOSlotHour(
        int ScheduleLessonId,
        int CurriculumId,
        ClassBookType BookType,
        int? BasicClassId,
        int ClassBookId,
        string ClassBookFullName,
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
        bool? IsEmptyHour,
        bool IsReplTeacher,
        bool? ReplTeacherIsNonSpecialist,
        string? Location);
}
