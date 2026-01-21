namespace SB.Domain;

public partial interface IClassBooksQueryRepository
{
    public record GetAllVO(
        int ClassBookId,
        ClassBookType BookType,
        string BookName,
        string FullBookName,
        int? BasicClassId,
        string? BasicClassName,
        int? BasicClassSortOrd,
        bool IsValid);
}
