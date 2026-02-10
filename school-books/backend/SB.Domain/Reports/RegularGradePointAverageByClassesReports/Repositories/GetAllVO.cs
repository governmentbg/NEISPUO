namespace SB.Domain;
using System;

public partial interface IRegularGradePointAverageByClassesReportsQueryRepository
{
    public record GetAllVO(
        int SchoolYear,
        int RegularGradePointAverageByClassesReportId,
        string Period,
        string? ClassBookNames,
        DateTime CreateDate);
}
