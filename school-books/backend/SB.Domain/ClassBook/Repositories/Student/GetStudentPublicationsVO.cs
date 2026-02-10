namespace SB.Domain;

using SB.Common;
using System;
using System.Text.Json.Serialization;

public partial interface IStudentPublicationsQueryRepository
{
    public record GetStudentPublicationsVO(
        int PublicationId,
        DateTime Date,
        string Title,
        [property: JsonConverter(typeof(HtmlTextWithLinksConverter))] string ContentHtml,
        bool HasAttachedFiles);
}
