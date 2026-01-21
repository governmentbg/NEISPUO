namespace SB.Domain;

using System.Text.Json.Serialization;

public record CreateSessionStudentsReportCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    [JsonIgnore] public string ObjectName => nameof(SessionStudentsReport);
    [JsonIgnore] public virtual int? ObjectId => null;
}
