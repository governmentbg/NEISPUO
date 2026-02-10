namespace SB.Domain;

using System;

public partial interface ISessionStudentsReportsQueryRepository
{
    public record GetVO(
        int SessionStudentsReportId,
        DateTime CreateDate);
}
