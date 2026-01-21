namespace SB.Domain;

using System.Text.Json.Serialization;
using MediatR;

public record RemoveExamsReportCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? ExamsReportId { get; init; }

    [JsonIgnore] public string ObjectName => nameof(ExamsReport);
    [JsonIgnore] public int? ObjectId => this.ExamsReportId;
}
