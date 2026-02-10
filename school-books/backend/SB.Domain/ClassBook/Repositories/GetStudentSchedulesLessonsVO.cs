namespace SB.Domain;

public partial interface IClassBookStudentPrintRepository
{
    public record GetStudentSchedulesLessonsVO(
        int ScheduleId,
        int Day,
        int HourNumber,
        string SubjectName,
        string SubjectTypeName);
}
