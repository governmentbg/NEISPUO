namespace SB.Domain;

using System;

public partial interface IScheduleAndAbsencesByTermAllClassesReportsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int ScheduleAndAbsencesByTermAllClassesReportId,
        SchoolTerm Term,
        string BlobDownloadUrl,
        DateTime CreateDate);
}
