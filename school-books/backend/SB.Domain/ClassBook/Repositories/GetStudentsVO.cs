namespace SB.Domain;

public partial interface IClassBooksQueryRepository
{
    public record GetStudentsVO(
        int PersonId,
        int? ClassNumber,
        string FirstName,
        string? MiddleName,
        string LastName,
        bool IsTransferred,
        bool HasSpecialNeedFirstGradeResult);
}
