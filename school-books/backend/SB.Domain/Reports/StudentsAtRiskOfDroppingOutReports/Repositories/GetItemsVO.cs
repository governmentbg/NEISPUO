namespace SB.Domain;

public partial interface IStudentsAtRiskOfDroppingOutReportsQueryRepository
{
    public record GetItemsVO(
        string PersonalId,
        string FirstName,
        string MiddleName,
        string LastName,
        string ClassBookName,
        decimal? UnexcusedAbsenceHoursCount,
        int? UnexcusedAbsenceDaysCount);
}
