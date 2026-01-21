namespace SB.Domain;

using System;

public partial interface IScheduleAndAbsencesByMonthReportsQueryRepository
{
    public record GetAllVO(
        int ScheduleAndAbsencesByMonthReportId,
        int SchoolYear,
        string YearAndMonth,
        string ClassBookName,
        DateTime CreateDate);
}
