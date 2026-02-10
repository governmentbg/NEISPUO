namespace SB.Domain;

using System.Text.Json.Serialization;

public record UpdateHighSchoolCertificateProtocolCommand : CreateHighSchoolCertificateProtocolCommand
{
    [JsonIgnore]public int? HighSchoolCertificateProtocolId { get; init; }

    [JsonIgnore]public override int? ObjectId => this.HighSchoolCertificateProtocolId;
}
