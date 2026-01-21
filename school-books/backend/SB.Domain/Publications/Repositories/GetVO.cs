namespace SB.Domain;

using System;

public partial interface IPublicationsQueryRepository
{
    public record GetVO(
        int PublicationId,
        PublicationType Type,
        PublicationStatus Status,
        DateTime Date,
        string Title,
        string Content,
        GetVOFile[] Files);

    public record GetVOFile(
        int BlobId,
        string FileName,
        string Location);
}
