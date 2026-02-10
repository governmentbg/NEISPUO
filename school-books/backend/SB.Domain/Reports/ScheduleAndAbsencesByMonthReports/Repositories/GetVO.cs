namespace SB.Domain;

using System;

public partial interface IScheduleAndAbsencesByMonthReportsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int ScheduleAndAbsencesByMonthReportId,
        int Year,
        int Month,
        string ClassBookName,
        bool IsDPLR,
        DateTime CreateDate);
}
