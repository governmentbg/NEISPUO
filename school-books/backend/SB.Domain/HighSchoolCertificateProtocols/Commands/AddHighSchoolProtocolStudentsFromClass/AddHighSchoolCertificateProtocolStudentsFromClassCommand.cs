namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record AddHighSchoolCertificateProtocolStudentsFromClassCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? HighSchoolCertificateProtocolId { get; init; }

    public int? ClassId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(HighSchoolCertificateProtocol);
    [JsonIgnore]public virtual int? ObjectId => null;
}
