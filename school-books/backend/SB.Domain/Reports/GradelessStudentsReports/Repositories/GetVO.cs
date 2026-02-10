namespace SB.Domain;

using System;

public partial interface IGradelessStudentsReportsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int GradelessStudentsReportId,
        bool OnlyFinalGrades,
        ReportPeriod? Period,
        string? ClassBookNames,
        DateTime CreateDate);
}
