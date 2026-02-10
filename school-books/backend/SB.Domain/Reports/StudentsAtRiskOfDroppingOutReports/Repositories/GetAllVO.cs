namespace SB.Domain;

using System;

public partial interface IStudentsAtRiskOfDroppingOutReportsQueryRepository
{
    public record GetAllVO(
        int StudentsAtRiskOfDroppingOutReportId,
        DateTime ReportDate,
        DateTime CreateDate);
}
