namespace SB.Domain;

public partial interface IClassBookCQSQueryRepository
{
    public record GetVO(
        int InstId,
        int ClassId,
        int? BasicClassId,
        bool ClassIsLvl2,
        ClassBookType BookType,
        bool IsFinalized,
        bool IsValid);
}
