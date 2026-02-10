namespace SB.Domain;

using System.Text.Json.Serialization;

public record UpdateShiftCommand : CreateShiftCommand
{
    [JsonIgnore]public int? ShiftId { get; init; }

    [JsonIgnore]public override int? ObjectId => this.ShiftId;
}
