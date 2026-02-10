namespace SB.Domain;
public partial interface ITopicPlansQueryRepository
{
    public record GetItemsVO(
        int TopicPlanItemId,
        int Number,
        string Title,
        string? Note);
}
