namespace SB.Domain;
using System;

public partial interface IFinalGradePointAverageByClassesReportsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int FinalGradePointAverageByClassesReportId,
        ReportPeriod Period,
        string? ClassBookNames,
        DateTime CreateDate);
}
