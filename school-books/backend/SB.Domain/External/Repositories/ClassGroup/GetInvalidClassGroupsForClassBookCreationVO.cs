namespace SB.Domain;

public partial interface IClassGroupsQueryRepository
{
    public record GetInvalidClassGroupsForClassBookCreationVO(
        int ClassId,
        string? ClassName,
        bool IsNonexistentClassGroup,
        bool IsValidFalseClassGroup,
        bool DuplicateClassBook,
        bool HasClassBookOnParentOrChildLevel,
        ClassBookTypeError? ClassBookTypeError);
}
