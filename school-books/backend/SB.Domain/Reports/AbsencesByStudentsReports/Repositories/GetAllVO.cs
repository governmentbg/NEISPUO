namespace SB.Domain;

using System;

public partial interface IAbsencesByStudentsReportsQueryRepository
{
    public record GetAllVO(
        int SchoolYear,
        int AbsencesByStudentsReportId,
        string Period,
        string? ClassBookNames,
        DateTime CreateDate);
}
