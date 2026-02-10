namespace SB.Domain;
public partial interface ITopicPlansQueryRepository
{
    public record GetItemVO(
        int Number,
        string Title,
        string? Note);
}
