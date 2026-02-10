namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveGradelessStudentsReportCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? GradelessStudentsReportId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(GradelessStudentsReport);
    [JsonIgnore]public int? ObjectId => this.GradelessStudentsReportId;
}
