namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record CreateHighSchoolCertificateProtocolStudentCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? HighSchoolCertificateProtocolId { get; init; }

    public CreateHighSchoolCertificateProtocolStudentCommandStudent[]? Students { get; init; }

    [JsonIgnore]public string ObjectName => nameof(HighSchoolCertificateProtocolStudent);
    [JsonIgnore]public virtual int? ObjectId => null;
}
