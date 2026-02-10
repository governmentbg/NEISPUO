namespace SB.Domain;
using System;

public partial interface IAbsencesByClassesReportsQueryRepository
{
    public record GetExcelDataVO(
        string Period,
        string? ClassBookNames,
        DateTime CreateDate,
        GetExcelDataVOItem[] Items);

    public record GetExcelDataVOItem(
        string ClassBookName,
        int StudentsCount,
        int ExcusedAbsencesCount,
        decimal ExcusedAbsencesCountAverage,
        decimal UnexcusedAbsencesCount,
        decimal UnexcusedAbsencesCountAverage,
        bool IsTotal);
}
