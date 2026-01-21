namespace SB.Domain;

public partial interface IAbsencesByStudentsReportsQueryRepository
{
    public record GetItemsVO(
        string ClassBookName,
        string StudentName,
        bool IsTransferred,
        int ExcusedAbsencesCount,
        decimal UnexcusedAbsencesCount,
        bool IsTotal);
}
