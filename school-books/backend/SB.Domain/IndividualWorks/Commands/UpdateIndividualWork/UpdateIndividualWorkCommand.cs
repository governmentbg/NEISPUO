namespace SB.Domain;

using System;
using System.Text.Json.Serialization;
using MediatR;

public record UpdateIndividualWorkCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? ClassBookId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? IndividualWorkId { get; init; }

    public DateTime? Date { get; init; }
    public string? IndividualWorkActivity { get; init; }

    [JsonIgnore]public string ObjectName => nameof(IndividualWork);
    [JsonIgnore]public int? ObjectId => this.IndividualWorkId;
}
