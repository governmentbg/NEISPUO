namespace SB.Domain;

using System;

public partial interface ISessionStudentsReportsQueryRepository
{
    public record GetAllVO(
        int SessionStudentsReportId,
        DateTime CreateDate);
}
