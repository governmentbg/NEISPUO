namespace SB.Domain;

using System;

public partial interface ILectureSchedulesReportsQueryRepository
{
    public record GetAllVO(
        int LectureSchedulesReportId,
        string Period,
        string? YearAndMonth,
        string? TeacherName,
        DateTime CreateDate);
}
