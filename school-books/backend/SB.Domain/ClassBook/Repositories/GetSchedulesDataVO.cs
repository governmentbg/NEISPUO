namespace SB.Domain;

using System;

public partial interface IClassBookPrintRepository
{
    public record GetSchedulesDataVO(
        int ScheduleId,
        int? StudentPersonId,
        string? StudentName,
        DateTime StartDate,
        DateTime EndDate);
}
