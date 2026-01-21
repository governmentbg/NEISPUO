namespace SB.Domain;

using System.Text.Json.Serialization;

public record UpdateStateExamsAdmProtocolCommand : CreateStateExamsAdmProtocolCommand
{
    [JsonIgnore]public int? StateExamsAdmProtocolId { get; init; }

    [JsonIgnore]public override int? ObjectId => this.StateExamsAdmProtocolId;
}
