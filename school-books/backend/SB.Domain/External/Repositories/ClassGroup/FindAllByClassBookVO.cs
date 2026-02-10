namespace SB.Domain;

public partial interface IStudentClassQueryRepository
{
    public record FindAllByClassBookVO(
        StudentClass StudentClass,
        int PersonId,
        string FirstName,
        string? MiddleName,
        string LastName);
}
