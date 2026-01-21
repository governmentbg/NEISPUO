namespace SB.Domain;
using System;

public partial interface IRegularGradePointAverageByClassesReportsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int RegularGradePointAverageByClassesReportId,
        ReportPeriod Period,
        string? ClassBookNames,
        DateTime CreateDate);
}
