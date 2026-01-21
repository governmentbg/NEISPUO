namespace SB.Domain;
using System;

public partial interface IAbsencesByClassesReportsQueryRepository
{
    public record GetAllVO(
        int SchoolYear,
        int AbsencesByClassesReportId,
        string Period,
        string? ClassBookNames,
        DateTime CreateDate);
}
