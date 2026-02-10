namespace SB.Domain;

using System.Text.Json.Serialization;

public record CreateExamsReportCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    [JsonIgnore] public string ObjectName => nameof(ExamsReport);
    [JsonIgnore] public virtual int? ObjectId => null;
}
