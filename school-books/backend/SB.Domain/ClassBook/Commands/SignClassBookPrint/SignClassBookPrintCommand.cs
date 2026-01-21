namespace SB.Domain;

using MediatR;
using System.IO;
using System.Text.Json.Serialization;

public record SignClassBookPrintCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public int? ClassBookId { get; init; }
    public int? ClassBookPrintId { get; init; }

    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore] // use Newtonsoft.Json.JsonIgnore to skip Stream serialization in the AuditBehavior
    public Stream? SignedClassBookPrintFileBase64 { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ClassBook);
    [JsonIgnore]public virtual int? ObjectId => null;
}
