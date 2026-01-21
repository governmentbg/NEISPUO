namespace SB.Domain;

using System;

public partial interface ILectureSchedulesReportsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int LectureSchedulesReportId,
        LectureSchedulesReportPeriod Period,
        int? Year,
        int? Month,
        string? TeacherPersonName,
        DateTime CreateDate);
}
