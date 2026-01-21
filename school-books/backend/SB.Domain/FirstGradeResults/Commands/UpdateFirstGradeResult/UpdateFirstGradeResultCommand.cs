namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record UpdateFirstGradeResultCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? ClassBookId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public UpdateFirstGradeResultCommandStudent[]? Students { get; init; }

    [JsonIgnore]public string ObjectName => nameof(FirstGradeResult);
    [JsonIgnore]public virtual int? ObjectId => null;
}
