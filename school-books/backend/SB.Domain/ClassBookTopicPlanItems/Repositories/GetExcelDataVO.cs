namespace SB.Domain;
public partial interface IClassBookTopicPlanItemsQueryRepository
{
    public record GetExcelDataVO(
        int Number,
        string Title,
        string? Note);
}
