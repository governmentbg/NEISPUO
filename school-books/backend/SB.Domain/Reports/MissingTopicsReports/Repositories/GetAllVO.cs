namespace SB.Domain;

using System;

public partial interface IMissingTopicsReportsQueryRepository
{
    public record GetAllVO(
        int MissingTopicsReportId,
        int SchoolYear,
        string Period,
        string? YearAndMonth,
        string? TeacherName,
        DateTime CreateDate);
}
