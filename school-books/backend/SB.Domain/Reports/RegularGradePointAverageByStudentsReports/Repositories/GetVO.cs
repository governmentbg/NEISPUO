namespace SB.Domain;
using System;

public partial interface IRegularGradePointAverageByStudentsReportsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int RegularGradePointAverageByStudentsReportId,
        ReportPeriod Period,
        string? ClassBookNames,
        DateTime CreateDate);
}
