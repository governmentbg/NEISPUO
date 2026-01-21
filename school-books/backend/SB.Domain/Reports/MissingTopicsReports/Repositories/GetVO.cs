namespace SB.Domain;

using System;

public partial interface IMissingTopicsReportsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int MissingTopicsReportId,
        MissingTopicsReportPeriod Period,
        int? Year,
        int? Month,
        int? TeacherPersonId,
        DateTime CreateDate);
}
