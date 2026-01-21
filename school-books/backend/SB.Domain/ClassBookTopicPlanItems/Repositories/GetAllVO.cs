namespace SB.Domain;

public partial interface IClassBookTopicPlanItemsQueryRepository
{
    public record GetAllVO(
        int ClassBookTopicPlanItemId,
        int Number,
        string Title,
        bool Taken,
        string? Note);
}
