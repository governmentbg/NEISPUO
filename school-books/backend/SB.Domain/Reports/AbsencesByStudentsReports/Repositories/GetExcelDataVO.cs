namespace SB.Domain;

using System;

public partial interface IAbsencesByStudentsReportsQueryRepository
{
    public record GetExcelDataVO(
        string Period,
        string? ClassBookNames,
        DateTime CreateDate,
        GetExcelDataVOItem[] Items);

    public record GetExcelDataVOItem(
        string ClassBookName,
        string StudentName,
        bool IsTransferred,
        int ExcusedAbsencesCount,
        decimal UnexcusedAbsencesCount,
        bool IsTotal);
}
