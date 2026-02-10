namespace SB.Domain;

using System;

public partial interface ISchedulesQueryRepository
{
    public record GetAllByClassBookVO(
        int SchoolYear,
        int ScheduleId,
        int ClassBookId,
        SchoolTerm? Term,
        bool IsRziApproved,
        int ShiftId,
        string ShiftName,
        string StudentNames,
        GetAllByClassBookVODateRange[] Dates);

    public record GetAllByClassBookVODateRange(
        DateTime StartDate,
        DateTime EndDate);
}
