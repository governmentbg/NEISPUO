namespace SB.Domain;

public partial interface IClassGroupsQueryRepository
{
    public record GetClassGroupsWIthClassBookVO(
        ClassGroup ClassGroup,
        ClassBook? ClassBook);
}
