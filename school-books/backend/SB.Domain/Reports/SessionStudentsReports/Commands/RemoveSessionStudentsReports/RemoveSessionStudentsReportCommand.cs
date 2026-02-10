namespace SB.Domain;

using System.Text.Json.Serialization;
using MediatR;

public record RemoveSessionStudentsReportCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? SessionStudentsReportId { get; init; }

    [JsonIgnore] public string ObjectName => nameof(SessionStudentsReport);
    [JsonIgnore] public int? ObjectId => this.SessionStudentsReportId;
}
