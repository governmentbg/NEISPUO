namespace SB.Domain;

using System;

public partial interface IExamsReportsQueryRepository
{
    public record GetVO(
        int ExamsReportId,
        DateTime CreateDate);
}
