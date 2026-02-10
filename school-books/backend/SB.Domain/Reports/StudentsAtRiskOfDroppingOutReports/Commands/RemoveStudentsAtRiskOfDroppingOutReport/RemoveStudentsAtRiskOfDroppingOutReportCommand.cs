namespace SB.Domain;

using System.Text.Json.Serialization;
using MediatR;

public record RemoveStudentsAtRiskOfDroppingOutReportCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? StudentsAtRiskOfDroppingOutReportId { get; init; }

    [JsonIgnore] public string ObjectName => nameof(StudentsAtRiskOfDroppingOutReport);
    [JsonIgnore] public int? ObjectId => this.StudentsAtRiskOfDroppingOutReportId;
}
