namespace SB.Domain;

using System;

public partial interface IClassBookStudentPrintRepository
{
    public record GetStudentSchedulesDataVO(
        int ScheduleId,
        DateTime StartDate,
        DateTime EndDate);
}
