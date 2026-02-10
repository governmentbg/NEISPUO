namespace SB.Domain;

public partial interface IClassBooksQueryRepository
{
    public record GetRemovedStudentsVO(
        int PersonId,
        string FirstName,
        string? MiddleName,
        string LastName);
}
