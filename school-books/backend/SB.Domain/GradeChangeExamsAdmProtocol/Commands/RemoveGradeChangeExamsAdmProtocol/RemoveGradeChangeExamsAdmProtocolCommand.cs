namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveGradeChangeExamsAdmProtocolCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    [JsonIgnore]public int? GradeChangeExamsAdmProtocolId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(GradeChangeExamsAdmProtocol);
    [JsonIgnore]public int? ObjectId => this.GradeChangeExamsAdmProtocolId;
}
