namespace SB.Domain;

public partial interface IClassBooksAdminQueryRepository
{
    public record GetStudentNumbersVO(
        int PersonId,
        int? ClassNumber,
        string StudentFullName);
}
