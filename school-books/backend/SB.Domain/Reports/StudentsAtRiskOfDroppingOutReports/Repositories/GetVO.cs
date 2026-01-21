namespace SB.Domain;

using System;

public partial interface IStudentsAtRiskOfDroppingOutReportsQueryRepository
{
    public record GetVO(
        int StudentsAtRiskOfDroppingOutReportId,
        DateTime ReportDate,
        DateTime CreateDate);
}
