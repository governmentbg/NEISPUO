namespace SB.Domain;

public partial interface IClassGroupsQueryRepository
{
    public record GetExtApiClassGroupsVO(
        ClassGroup ClassGroup,
        BasicClass? BasicClass,
        ClassType? ClassType);
}
