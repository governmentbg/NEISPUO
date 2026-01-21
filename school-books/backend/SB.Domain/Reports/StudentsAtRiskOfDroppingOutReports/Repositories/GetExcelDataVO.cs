namespace SB.Domain;

using System;

public partial interface IStudentsAtRiskOfDroppingOutReportsQueryRepository
{
    public record GetExcelDataVO(
        DateTime ReportDate,
        DateTime CreateDate,
        GetExcelDataVOItem[] ReportItems);

    public record GetExcelDataVOItem(
        string PersonalId,
        string FirstName,
        string MiddleName,
        string LastName,
        string ClassBookName,
        decimal? UnexcusedAbsenceHoursCount,
        int? UnexcusedAbsenceDaysCount);
}
