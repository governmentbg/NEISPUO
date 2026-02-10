namespace SB.Domain;

using System.Text.Json.Serialization;

public record UpdateOffDayCommand : CreateOffDayCommand
{
    [JsonIgnore]public int? OffDayId { get; init; }

    [JsonIgnore]public override int? ObjectId => this.OffDayId;
}
