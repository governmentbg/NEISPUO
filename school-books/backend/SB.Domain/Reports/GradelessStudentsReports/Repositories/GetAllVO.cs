namespace SB.Domain;

using System;

public partial interface IGradelessStudentsReportsQueryRepository
{
    public record GetAllVO(
        int SchoolYear,
        int GradelessStudentsReportId,
        bool OnlyFinalGrades,
        string? Period,
        string? ClassBookNames,
        DateTime CreateDate);
}
