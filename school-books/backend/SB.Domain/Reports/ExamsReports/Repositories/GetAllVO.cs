namespace SB.Domain;

using System;

public partial interface IExamsReportsQueryRepository
{
    public record GetAllVO(
        int ExamsReportId,
        DateTime CreateDate);
}
