namespace SB.Domain;

public partial interface IClassBookTopicPlanItemsQueryRepository
{
    public record GetVO(
        int Number,
        string Title,
        bool Taken,
        string? Note);
}
