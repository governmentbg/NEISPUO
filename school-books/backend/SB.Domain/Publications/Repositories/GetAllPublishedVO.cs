namespace SB.Domain;

using SB.Common;
using System;
using System.Text.Json.Serialization;

public partial interface IPublicationsQueryRepository
{
    public record GetAllPublishedVO(
        int PublicationId,
        DateTime Date,
        string Title,
        [property: JsonConverter(typeof(HtmlTextWithLinksConverter))] string ContentHtml,
        bool IsInternal,
        bool HasAttachedFiles);
}
