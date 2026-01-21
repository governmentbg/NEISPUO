namespace SB.Domain;
public partial interface IClassBookPrintRepository
{
    public record GetSchedulesLessonsVO(
        int ScheduleId,
        int Day,
        int HourNumber,
        string SubjectName,
        string SubjectTypeName);
}
