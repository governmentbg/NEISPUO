namespace SB.Domain;
public partial interface IPublicationsQueryRepository
{
    public record GetMetadataVO(
        int Count,
        int ArchivedCount);
}
