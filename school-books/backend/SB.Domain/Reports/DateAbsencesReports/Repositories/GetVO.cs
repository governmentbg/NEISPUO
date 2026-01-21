namespace SB.Domain;

using System;

public partial interface IDateAbsencesReportsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int DateAbsencesReportId,
        DateTime ReportDate,
        bool IsUnited,
        string? ClassBookNames,
        string? ShiftNames,
        DateTime CreateDate);
}
