namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveRemarkCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? ClassBookId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    [JsonIgnore] public bool IsExternal { get; init; }
    [JsonIgnore] public int? CurriculumId { get; init; }
    [JsonIgnore] public int? RemarkId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Remark);
    [JsonIgnore]public int? ObjectId => this.RemarkId;
}
