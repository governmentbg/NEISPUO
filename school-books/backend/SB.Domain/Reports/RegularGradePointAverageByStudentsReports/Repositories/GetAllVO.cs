namespace SB.Domain;
using System;

public partial interface IRegularGradePointAverageByStudentsReportsQueryRepository
{
    public record GetAllVO(
        int SchoolYear,
        int RegularGradePointAverageByStudentsReportId,
        string Period,
        string? ClassBookNames,
        DateTime CreateDate);
}
