namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveStateExamsAdmProtocolStudentCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? StateExamsAdmProtocolId { get; init; }

    [JsonIgnore]public int? ClassId { get; init; }
    [JsonIgnore]public int? PersonId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(StateExamsAdmProtocolStudent);
    [JsonIgnore]public virtual int? ObjectId => null;
}
