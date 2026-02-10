namespace SB.Domain;

public partial interface IStudentPublicationsQueryRepository
{
    public record GetStudentPublicationsMetadataVO(
        string InstName,
        int Count,
        int ArchivedCount);
}
