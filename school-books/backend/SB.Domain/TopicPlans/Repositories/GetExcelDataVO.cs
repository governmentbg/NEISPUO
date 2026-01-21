namespace SB.Domain;
public partial interface ITopicPlansQueryRepository
{
    public record GetExcelDataVO(
        int Number,
        string Title,
        string? Note);
}
