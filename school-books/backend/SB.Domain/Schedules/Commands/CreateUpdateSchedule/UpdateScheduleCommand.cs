namespace SB.Domain;

using System.Text.Json.Serialization;

public record UpdateScheduleCommand : CreateScheduleCommand
{
    [JsonIgnore]public int? ScheduleId { get; init; }

    [JsonIgnore]public override int? ObjectId => this.ScheduleId;
}
