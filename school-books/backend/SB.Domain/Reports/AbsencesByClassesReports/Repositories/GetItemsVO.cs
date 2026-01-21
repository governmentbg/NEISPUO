namespace SB.Domain;

public partial interface IAbsencesByClassesReportsQueryRepository
{
    public record GetItemsVO(
        string ClassBookName,
        int StudentsCount,
        int ExcusedAbsencesCount,
        decimal ExcusedAbsencesCountAverage,
        decimal UnexcusedAbsencesCount,
        decimal UnexcusedAbsencesCountAverage,
        bool IsTotal);
}
