namespace SB.Domain;

public partial interface IClassBooksAdminQueryRepository
{
    public record GetVO(
        int ClassBookId,
        string ClassName,
        string? BasicClassName,
        string BookName,
        ClassBookType BookType,
        int? BasicClassId,
        bool IsFinalized,
        bool IsValid);
}
