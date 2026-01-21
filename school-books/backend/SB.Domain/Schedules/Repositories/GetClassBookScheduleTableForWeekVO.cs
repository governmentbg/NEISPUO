namespace SB.Domain;

public partial interface ISchedulesQueryRepository
{
    public record GetClassBookScheduleTableForWeekVO(
        GetClassBookScheduleTableForWeekVOShiftHour[] ShiftHours,
        GetClassBookScheduleTableForWeekVOSlot[] Slots,
        int[] OffDays);

    public record GetClassBookScheduleTableForWeekVOShiftHour(
        int SlotNumber,
        string StartTime,
        string EndTime);

    public record GetClassBookScheduleTableForWeekVOSlot(
        int Day,
        int SlotNumber,
        GetClassBookScheduleTableForWeekVOSlotHour[] Hours);

    public record GetClassBookScheduleTableForWeekVOSlotHour(
        int ScheduleLessonId,
        int CurriculumId,
        string? CurriculumGroupName,
        string SubjectName,
        string? SubjectNameShort,
        string SubjectTypeName,
        bool IsIndividualLesson,
        bool IsIndividualCurriculum,
        GetClassBookScheduleTableForWeekVOSlotHourTeacher[] CurriculumTeachers,
        int? TeacherAbsenceId,
        GetClassBookScheduleTableForWeekVOSlotHourTeacher? ReplTeacher,
        bool? ReplTeacherIsNonSpecialist,
        string? ExtTeacherName,
        bool? IsEmptyHour,
        string? Location);

    public record GetClassBookScheduleTableForWeekVOSlotHourTeacher(
        int TeacherPersonId,
        string TeacherFirstName,
        string TeacherLastName,
        bool MarkedAsNoReplacement);
}
