namespace SB.Domain;

public partial interface IStudentInfoClassBooksQueryRepository
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
        bool IsValid,
        string FirstName,
        string? MiddleName,
        string LastName,
        bool IsTransferred);
}
