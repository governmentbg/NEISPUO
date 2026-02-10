namespace SB.Domain;
using System;

public partial interface IFinalGradePointAverageByStudentsReportsQueryRepository
{
    public record GetAllVO(
        int SchoolYear,
        int FinalGradePointAverageByStudentsReportId,
        string Period,
        string? ClassBookNames,
        decimal? MinimumGradePointAverage,
        DateTime CreateDate);
}
