namespace SB.Domain;

using SB.Common;

public partial interface IClassBooksAdminQueryRepository
{
    public record GetAllVO(
        int ClassBookId,
        string ClassName,
        string? BasicClassName,
        string BookName,
        ClassBookType BookType,
        bool IsValid)
    {
        public string? BookTypeDescription => this.BookType.GetEnumDescription();
    }
}
