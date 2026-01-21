namespace SB.Domain;

public partial interface IDateAbsencesReportsQueryRepository
{
    public record GetItemsForAddVO(
        int ClassBookId,
        string ClassBookName,
        int? ShiftId,
        string? ShiftName,
        int HourNumber,
        string? AbsenceStudentNumbers,
        int AbsenceStudentCount,
        bool IsOffDay,
        bool HasScheduleDate);
}
