namespace SB.Domain;

public partial interface IDateAbsencesReportsQueryRepository
{
    public record GetItemsVO(
        int ClassBookId,
        string ClassBookName,
        bool IsOffDay,
        bool HasScheduleDate,
        GetItemsVOHour[] Hours);

    public record GetItemsVOHour(
        int? ShiftId,
        string? ShiftName,
        int HourNumber,
        string? AbsenceStudentNumbers,
        int AbsenceStudentCount);
}
