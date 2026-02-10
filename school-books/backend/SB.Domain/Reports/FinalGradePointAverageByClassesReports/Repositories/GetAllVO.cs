namespace SB.Domain;
using System;

public partial interface IFinalGradePointAverageByClassesReportsQueryRepository
{
    public record GetAllVO(
        int SchoolYear,
        int FinalGradePointAverageByClassesReportId,
        string Period,
        string? ClassBookNames,
        DateTime CreateDate);
}
