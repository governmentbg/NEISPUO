namespace SB.Domain;

using SB.Common;

public partial interface IClassGroupsQueryRepository
{
    public record GetAllForClassKindVO(
        int ClassId,
        string ClassName,
        int? ParentClassId,
        string? BasicClassName,
        string? ClassTypeName,
        bool IsLvl1Combined,
        bool ClassIsLvl2,
        int? ClassBookId,
        string? ClassBookName,
        ClassBookType? BookType,
        ClassBookTypeError? BookTypeError,
        string? SuggestedClassBookName)
    {
        public string? BookTypeDescription => this.BookType?.GetEnumDescription();
        public string? BookTypeErrorDescription => this.BookTypeError?.GetEnumDescription();
    }
}
