namespace SB.Domain;

public partial interface IStudentClassBooksQueryRepository
{
    public record GetAllClassBooksVO(
        int SchoolYear,
        int InstId,
        string InstName,
        int ClassBookId,
        int PersonId,
        ClassBookType BookType,
        string BookName,
        int? BasicClassId,
        string? BasicClassName,
        string FirstName,
        string? MiddleName,
        string LastName,
        bool IsTransferred);
}
