namespace SB.Domain;

using System;

public partial interface IPublicationsQueryRepository
{
    public record GetAllVO(
        int PublicationId,
        string Type,
        string Status,
        DateTime Date,
        string Title,
        string Content);
}
