namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveDateAbsencesReportCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? DateAbsencesReportId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(DateAbsencesReport);
    [JsonIgnore]public int? ObjectId => this.DateAbsencesReportId;
}
