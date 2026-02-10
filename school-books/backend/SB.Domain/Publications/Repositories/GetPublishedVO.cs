namespace SB.Domain;

using SB.Common;
using System;
using System.Text.Json.Serialization;

public partial interface IPublicationsQueryRepository
{
    public record GetPublishedVO(
        int PublicationId,
        DateTime Date,
        string Title,
        [property: JsonConverter(typeof(HtmlTextWithLinksConverter))] string ContentHtml,
        GetPublishedVOFile[] Files);

    public record GetPublishedVOFile(
        string FileName,
        string Location,
        string Extension);
}
