namespace SB.Domain;

using System;

public partial interface IScheduleAndAbsencesByTermReportsQueryRepository
{
    public record GetAllVO(
        int ScheduleAndAbsencesByTermReportId,
        int SchoolYear,
        string Term,
        string ClassBookName,
        DateTime CreateDate);
}
