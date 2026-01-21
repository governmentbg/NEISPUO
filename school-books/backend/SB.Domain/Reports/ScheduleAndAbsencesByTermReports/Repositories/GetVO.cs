namespace SB.Domain;

using System;

public partial interface IScheduleAndAbsencesByTermReportsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int ScheduleAndAbsencesByTermReportId,
        SchoolTerm Term,
        string ClassBookName,
        bool IsDPLR,
        DateTime CreateDate);
}
