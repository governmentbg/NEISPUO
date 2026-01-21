namespace SB.Domain;

public partial interface IStudentInfoClassBooksQueryRepository
{
    public record GetAllStudentsVO(
        int PersonId,
        string FirstName,
        string? MiddleName,
        string LastName);
}
