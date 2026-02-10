namespace SB.Domain;

using SB.Common;
using System;
using System.Text.Json.Serialization;

public partial interface IStudentPublicationsQueryRepository
{
    public record GetStudentPublicationVO(
        int PublicationId,
        DateTime Date,
        string Title,
        [property: JsonConverter(typeof(HtmlTextWithLinksConverter))] string ContentHtml,
        GetStudentPublicationVOFile[] Files);

    public record GetStudentPublicationVOFile(
        string FileName,
        string Location,
        string Extension);
}
