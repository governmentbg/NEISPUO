namespace SB.Domain;

using System;

public partial interface IScheduleAndAbsencesByTermAllClassesReportsQueryRepository
{
    public record GetAllVO(
        int ScheduleAndAbsencesByTermAllClassesReportId,
        int SchoolYear,
        string Term,
        DateTime CreateDate);
}
