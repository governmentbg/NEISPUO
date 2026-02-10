namespace SB.Domain;
using System;

public partial interface IFinalGradePointAverageByStudentsReportsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int FinalGradePointAverageByStudentsReportId,
        ReportPeriod Period,
        string? ClassBookNames,
        decimal? MinimumGradePointAverage,
        DateTime CreateDate);
}
