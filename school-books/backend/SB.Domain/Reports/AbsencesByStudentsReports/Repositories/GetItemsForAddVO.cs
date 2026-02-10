namespace SB.Domain;

public partial interface IAbsencesByStudentsReportsQueryRepository
{
    public record GetItemsForAddVO(
        string ClassBookName,
        string StudentName,
        bool IsTransferred,
        int ExcusedAbsencesCount,
        int UnexcusedAbsencesCount,
        int LateAbsencesCount,
        bool IsTotal);
}
