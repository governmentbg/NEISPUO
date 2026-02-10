namespace SB.Domain;
using System;

public partial interface IAbsencesByClassesReportsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int AbsencesByClassesReportId,
        ReportPeriod Period,
        string? ClassBookNames,
        DateTime CreateDate);
}
