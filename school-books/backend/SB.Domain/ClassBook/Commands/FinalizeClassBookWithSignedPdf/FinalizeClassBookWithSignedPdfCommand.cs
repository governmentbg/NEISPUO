namespace SB.Domain;

using MediatR;
using System.IO;
using System.Text.Json.Serialization;

public record FinalizeClassBookWithSignedPdfCommand : IRequest<int>, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? ClassBookId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore] // use Newtonsoft.Json.JsonIgnore to skip Stream serialization in the AuditBehavior
    public Stream? SignedClassBookPrintFile { get; init; }

    [JsonIgnore] public string? SignedClassBookPrintFileName { get; init; }
    [JsonIgnore] public bool? ExtractClassBookIdFromMetadataOrFileName { get; init; }

    [JsonIgnore] public string ObjectName => nameof(ClassBook);
    [JsonIgnore] public virtual int? ObjectId => null;
}
