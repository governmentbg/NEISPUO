namespace SB.Domain;

using System;

public partial interface IAbsencesByStudentsReportsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int AbsencesByStudentsReportId,
        ReportPeriod Period,
        string? ClassBookNames,
        DateTime CreateDate);
}
