namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record UpdateGradeResultSessionsCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? ClassBookId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public UpdateGradeResultSessionsCommandSession[]? Sessions { get; init; }

    [JsonIgnore]public string ObjectName => nameof(GradeResult);
    [JsonIgnore]public virtual int? ObjectId => null;
}
